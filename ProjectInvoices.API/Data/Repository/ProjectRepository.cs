using Microsoft.EntityFrameworkCore;
using TakalNew.Data;
using TaklaNew.API.Domain;
using TaklaNew.API.Domain.IRepository;
using TaklaNew.API.Utilities;

namespace TaklaNew.API.Data.Repository
{
    /// <summary>
    /// Concrete implementation of IProjectRepository
    /// </summary>
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task AddProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteProjectAsync(Project project)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<IList<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects.SingleOrDefaultAsync(x => x.Id == id);
        }

        /// <inheritdoc/>
        public async Task<IList<Project>> GetProjectsAsync(int page, int pageSize, string? search)
        {
            var query = _context.Projects.AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
            }

            query = query.Paginate(page, pageSize);

            return await query.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<int> GetTotalRecords(string? search)
        {
            var query = _context.Projects.AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
            }

            return await query.CountAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> IsExistingProjectAsync(string name)
        {
            return await _context.Projects.Where(x => x.Name == name).AnyAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateProjectAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
