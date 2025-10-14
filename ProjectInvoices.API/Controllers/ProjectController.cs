using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaklaNew.API.Domain;
using TaklaNew.API.Domain.IRepository;
using TaklaNew.API.Dtos;

namespace TaklaNew.API.Controllers
{
    /// <summary>
    /// Handles operations related to projects
    /// </summary>
    [Route("api/project")]
    [ApiController]
    [Authorize(Roles = "admin,setup entry")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _repository;
        private readonly IMapper _mapper;

        public ProjectController(IProjectRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all projects
        /// </summary>
        /// <returns>List of project dto objects</returns>
        /// <response code="200">Returns the List of project dto objects</response>
        /// 
        [HttpGet("all")]
        public async Task<ActionResult<List<ProjectDto>>> GetAll()
        {
            //Get all projects from DB
            var projects = await _repository.GetAllProjectsAsync();
            var projectsDto = _mapper.Map<IEnumerable<ProjectDto>>(projects);

            return Ok(projectsDto);
        }

        /// <summary>
        /// Retrieves projects after pagination and searching
        /// </summary>
        /// <param name="pageNumber">page number</param>
        /// <param name="pageSize">page size</param>
        /// <param name="search">search keyword</param>
        /// <returns>A dto object contains list of projects and total number of projects
        /// to be used for pagination
        /// </returns>
        /// <response code="200">Returns a dto object contains list of projects and total number of projects</response>
        /// 
        [HttpGet("search")]
        public async Task<ActionResult<ProjectsPaginateDto>> Get([FromQuery] int pageNumber, int pageSize, string? search = null)
        {
            //Get total number of projects in DB (for pagination)
            var totalRecords = await _repository.GetTotalRecords(search);

            //Get projects by page and search keyword
            var projects = await _repository.GetProjectsAsync(pageNumber, pageSize, search);

            var projectsDto = _mapper.Map<IEnumerable<ProjectDto>>(projects);
            var projectsPaginateDto = new ProjectsPaginateDto { Projects = projectsDto, TotalRecords = totalRecords };

            return Ok(projectsPaginateDto);
        }

        /// <summary>
        /// Retrieves a project by its id
        /// </summary>
        /// <param name="id">project id</param>
        /// <returns>the matching project dto object</returns>
        /// <response code="404">project not found</response>
        /// <response code="200">returns the matching project dto object</response>
        ///
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectUpdateGetDto>> Get(int id)
        {
            //Get project from DB by its id
            var project = await _repository.GetProjectByIdAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            var projectUpdateGetDto = _mapper.Map<ProjectUpdateGetDto>(project);
            return Ok(projectUpdateGetDto);
        }

        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="projectDto">project info</param>
        /// <returns>no content if project created successfully</returns>
        /// <response code="400">project info are not valid or project name already exists</response>
        /// <response code="204">project created successfully</response>
        ///
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProjectCreationDto projectDto)
        {
            //Check if new project name exists in DB
            var isExistingProject = await _repository.IsExistingProjectAsync(projectDto.Name);
            if (isExistingProject)
            {
                return BadRequest("The Project name is already in use");
            }

            var project = _mapper.Map<Project>(projectDto);

            //Add new project to DB
            await _repository.AddProjectAsync(project);
            
            return NoContent();
        }

        /// <summary>
        /// Update a project
        /// </summary>
        /// <param name="id">project id</param>
        /// <param name="projectDto">project info</param>
        /// <returns>no content if project updated successfully</returns>
        /// <response code="404">project not found</response>
        /// <response code="400">project info are not valid or project name already in use</response>
        /// <response code="204">project updated successfully</response>
        ///
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProjectUpdateDto projectDto)
        {
            //Get project from DB by its id
            var project = await _repository.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            if (!projectDto.Name.Equals(project.Name))
            {
                //Check if the project to update new name exists in DB
                var isExistingProject = await _repository.IsExistingProjectAsync(projectDto.Name);
                if (isExistingProject)
                {
                    return BadRequest("The Project name is already in use");
                }
            }

            _mapper.Map(projectDto, project);

            //Update project in DB
            await _repository.UpdateProjectAsync();
            return NoContent();
        }

        /// <summary>
        /// Delete a project
        /// </summary>
        /// <param name="id">project id</param>
        /// <returns>no content if project deleted successfully</returns>
        /// <response code="404">project not found</response>
        /// <response code="204">project deleted successfully</response>
        ///
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            //Get project from DB by its id
            var project = await _repository.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            //Delete project from DB
            await _repository.DeleteProjectAsync(project);
            return NoContent();
        }

        /// <summary>
        /// Retrieves a list of projects as a list of keyvalue pairs
        /// </summary>
        /// <returns>a list of projects as a list of keyvalue pairs</returns>
        /// <response code="200">a list of projects as a list of keyvalue pairs</response>
        ///
        [HttpGet("projectskeyvalue")]
        public async Task<ActionResult<List<KeyValueDto>>> GetProjectsKeyValue()
        {
            //Get all projects from DB
            var projects = await _repository.GetAllProjectsAsync();
            var result = projects.Select(x => new KeyValueDto { Key = x.Id.ToString(), Value = x.Name.ToString() });
            return Ok(result);
        }
    }
}
