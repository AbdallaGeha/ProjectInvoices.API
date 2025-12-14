using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectInvoices.API.Data;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Exceptions;
using ProjectInvoices.API.Services.Base;
using ProjectInvoices.API.Services.Interfaces;
using ProjectInvoices.API.Utilities;

namespace ProjectInvoices.API.Services
{
    public class ProjectService : NamedEntityService<Project>, IProjectService
    {
        private readonly IMapper _mapper;
        public ProjectService(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }
        public async Task AddProjectAsync(ProjectCreationDto Project)
        {
            await EnsureNameUniqueAsync(Project.Name, null);
            var ProjectEntity = _mapper.Map<Project>(Project);
            _context.Projects.Add(ProjectEntity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProjectAsync(int id)
        {
            var Project = await GetAsync(id);
            _context.Projects.Remove(Project);
            await _context.SaveChangesAsync();
        }

        public async Task<ProjectUpdateGetDto> GetProjectByIdAsync(int id)
        {
            var Project = await _context.Projects
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);

            if (Project == null)
                throw new NotFoundException("Project not found.");

            return _mapper.Map<ProjectUpdateGetDto>(Project);
        }

        public async Task<ProjectsPaginateDto> GetProjectsAsync(int page, int pageSize, string? search)
        {
            var query = _context.Projects.AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
            }

            var count = await query.CountAsync();
            var Projects = await query.Paginate(page, pageSize).ToListAsync();

            var ProjectsDto = _mapper.Map<IEnumerable<ProjectDto>>(Projects);
            var ProjectsPaginateDto = new ProjectsPaginateDto { Projects = ProjectsDto, TotalRecords = count };

            return ProjectsPaginateDto;
        }

        public async Task UpdateProjectAsync(int id, ProjectUpdateDto Project)
        {
            var ProjectEntity = await GetAsync(id);
            await EnsureNameUniqueAsync(Project.Name, id);
            _mapper.Map(Project, ProjectEntity);
            await _context.SaveChangesAsync();
        }
    }
}
