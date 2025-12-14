using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Services.Interfaces;

namespace ProjectInvoices.API.Controllers
{
    /// <summary>
    /// Handles operations related to Suppliers
    /// </summary>
    [Route("api/supplier")]
    [ApiController]
    [Authorize(Roles = "admin,setup entry")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SupplierController(ISupplierService service, ILookupService lookupService)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a paginated list of Suppliers based on the specified page number, page size,
        /// and optional search term.
        /// </summary>
        /// <response code="200">Returns the paginated list of Suppliers.</response>
        /// <response code="400">Returned when the request parameters are invalid.</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(SuppliersPaginateDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<SuppliersPaginateDto>> Get([FromQuery] int pageNumber, int pageSize, string? search = null)
        {
            var SuppliersPaginateDto = await _service.GetSuppliersAsync(pageNumber, pageSize, search);
            return Ok(SuppliersPaginateDto);
        }


        /// <summary>
        /// Gets a Supplier by ID.
        /// </summary>
        /// <response code="200">Returns the Supplier data.</response>
        /// <response code="404">Supplier not found.</response>
        /// <response code="400">Invalid Id supplied.</response>
        [ProducesResponseType(typeof(SupplierUpdateGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SupplierUpdateGetDto>> Get(int id)
        {
            var SupplierUpdateGetDto = await _service.GetSupplierByIdAsync(id);
            return Ok(SupplierUpdateGetDto);
        }

        /// <summary>
        /// Creates a new Supplier using the provided data.
        /// </summary>
        /// <response code="204">The Supplier was successfully created.</response>
        /// <response code="400">The request data is invalid.</response>
        /// <response code="409">A Supplier with the same name already exists.</response>
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Post([FromBody] SupplierCreationDto SupplierDto)
        {
            await _service.AddSupplierAsync(SupplierDto);
            return NoContent();
        }

        /// <summary>
        /// Updates an existing Supplier with the specified identifier.
        /// </summary>
        /// <response code="204">The Supplier was successfully updated.</response>
        /// <response code="400">The request data is invalid.</response>
        /// <response code="404">The Supplier with the given ID was not found.</response>
        /// <response code="409">Supplier name already exists.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Put(int id, [FromBody] SupplierUpdateDto SupplierDto)
        {
            await _service.UpdateSupplierAsync(id, SupplierDto);
            return NoContent();
        }

        /// <summary>
        /// Deletes a Supplier by its identifier.
        /// </summary>
        /// <response code="204">The Supplier was successfully deleted.</response>
        /// <response code="404">The Supplier with the specified ID was not found.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteSupplierAsync(id);
            return NoContent();
        }
    }
}
