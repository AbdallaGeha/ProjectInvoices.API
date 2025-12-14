using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Services.Interfaces;

namespace ProjectInvoices.API.Controllers
{
    /// <summary>
    /// Handles operations related to Projects
    /// </summary>
    [Route("api/project")]
    [ApiController]
    [Authorize(Roles = "admin,setup entry")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _service;

        public ProjectController(IProjectService service, ILookupService lookupService)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a paginated list of Projects based on the specified page number, page size,
        /// and optional search term.
        /// </summary>
        /// <response code="200">Returns the paginated list of Projects.</response>
        /// <response code="400">Returned when the request parameters are invalid.</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ProjectsPaginateDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ProjectsPaginateDto>> Get([FromQuery] int pageNumber, int pageSize, string? search = null)
        {
            var ProjectsPaginateDto = await _service.GetProjectsAsync(pageNumber, pageSize, search);
            return Ok(ProjectsPaginateDto);
        }


        /// <summary>
        /// Gets a Project by ID.
        /// </summary>
        /// <response code="200">Returns the Project data.</response>
        /// <response code="404">Project not found.</response>
        /// <response code="400">Invalid Id supplied.</response>
        [ProducesResponseType(typeof(ProjectUpdateGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectUpdateGetDto>> Get(int id)
        {
            var ProjectUpdateGetDto = await _service.GetProjectByIdAsync(id);
            return Ok(ProjectUpdateGetDto);
        }

        /// <summary>
        /// Creates a new Project using the provided data.
        /// </summary>
        /// <response code="204">The Project was successfully created.</response>
        /// <response code="400">The request data is invalid.</response>
        /// <response code="409">A Project with the same name already exists.</response>
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Post([FromBody] ProjectCreationDto ProjectDto)
        {
            await _service.AddProjectAsync(ProjectDto);
            return NoContent();
        }

        /// <summary>
        /// Updates an existing Project with the specified identifier.
        /// </summary>
        /// <response code="204">The Project was successfully updated.</response>
        /// <response code="400">The request data is invalid.</response>
        /// <response code="404">The Project with the given ID was not found.</response>
        /// <response code="409">Project name already exists.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Put(int id, [FromBody] ProjectUpdateDto ProjectDto)
        {
            await _service.UpdateProjectAsync(id, ProjectDto);
            return NoContent();
        }

        /// <summary>
        /// Deletes a Project by its identifier.
        /// </summary>
        /// <response code="204">The Project was successfully deleted.</response>
        /// <response code="404">The Project with the specified ID was not found.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteProjectAsync(id);
            return NoContent();
        }
    }
}
