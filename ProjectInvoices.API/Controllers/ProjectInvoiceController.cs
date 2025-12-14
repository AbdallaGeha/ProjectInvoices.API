using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Services.Interfaces;

namespace ProjectInvoices.API.Controllers
{
    /// <summary>
    /// Handles operations related to project invoices, including:
    /// add new invoice, update existing invoice, approve invoice
    /// </summary>
    [Route("api/projectinvoice")]
    [ApiController]
    [Authorize(Roles = "admin,invoice entry,payment entry")]
    public class ProjectInvoiceController : ControllerBase
    {
        private readonly IProjectInvoicesService _service;
        public ProjectInvoiceController(IProjectInvoicesService service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a new project invoice
        /// </summary>
        /// <response code="400">invoice info are not valid</response>
        /// <response code="204">invoice created successfully</response>
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody] ProjectInvoiceCreationDto projectInvoiceDto)
        {
            await _service.AddProjectInvoiceAsync(projectInvoiceDto);
            return NoContent();
        }

        /// <summary>
        /// Retrieves a project invoice by its id
        /// </summary>
        /// <response code="404">project invoice not found</response>
        /// <response code="200">returns the matching project invoice dto object</response>
        [HttpGet("putget/{id:int}")]
        [ProducesResponseType(typeof(ProjectInvoiceUpdateGetDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProjectInvoiceUpdateGetDto>> PutGet(int id)
        {
            var result = await _service.GetProjectInvoiceWithItemsByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// update a project invoice
        /// </summary>
        /// <response code="404">project invoice not found</response>
        /// <response code="400">project invoice info are not valid</response>
        /// <response code="204">project invoice updated successfully</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(int id, [FromBody] ProjectInvoiceUpdateDto projectInvoiceUpdateDto)
        {
            await _service.UpdateProjectInvoiceAsync(id, projectInvoiceUpdateDto);
            return NoContent();
        }

        /// <summary>
        /// Approve a project invoice
        /// </summary>
        /// <response code="404">project invoice not found</response>
        /// <response code="204">project invoice approved successfully</response>
        [HttpPut("approve/{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Approve(int id)
        {
            await _service.ApproveAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Retrieves list of project invoices detailed info by paging and searching criteria
        /// </summary>
        /// <response code="200">returns a list of project invoices detailed info</response>
        [HttpPost("projectinvoiceview")]
        [ProducesResponseType(typeof(ProjectInvoiceViewResponseDto), 200)]
        public async Task<ActionResult<ProjectInvoiceViewResponseDto>> GetProjectInvoiceView
            ([FromBody] ProjectInvoiceViewRequestDto requestDto)
        {
            var result = await _service.GetProjectInvoiceView(requestDto);
            return Ok(result);
        }
    }
}
