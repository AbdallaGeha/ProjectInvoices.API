using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Services.Interfaces;

namespace ProjectInvoices.API.Controllers
{
    /// <summary>
    /// Handles operations related to banks
    /// </summary>
    [Route("api/bank")]
    [ApiController]
    [Authorize(Roles = "admin,setup entry")]
    public class BankController : ControllerBase
    {
        private readonly IBankService _service;
        
        public BankController(IBankService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a paginated list of banks based on the specified page number, page size,
        /// and optional search term.
        /// </summary>
        /// <response code="200">Returns the paginated list of banks.</response>
        /// <response code="400">Returned when the request parameters are invalid.</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(BanksPaginateDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BanksPaginateDto>> Get([FromQuery] int pageNumber, int pageSize, string? search = null)
        {
            var banksPaginateDto = await _service.GetBanksAsync(pageNumber, pageSize, search);
            return Ok(banksPaginateDto);
        }


        /// <summary>
        /// Gets a bank by ID.
        /// </summary>
        /// <response code="200">Returns the bank data.</response>
        /// <response code="404">Bank not found.</response>
        /// <response code="400">Invalid Id supplied.</response>
        [ProducesResponseType(typeof(BankUpdateGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BankUpdateGetDto>> Get(int id)
        {
            var bankUpdateGetDto = await _service.GetBankByIdAsync(id);
            return Ok(bankUpdateGetDto);
        }

        /// <summary>
        /// Creates a new bank using the provided data.
        /// </summary>
        /// <response code="204">The bank was successfully created.</response>
        /// <response code="400">The request data is invalid.</response>
        /// <response code="409">A bank with the same name already exists.</response>
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Post([FromBody] BankCreationDto bankDto)
        {
            await _service.AddBankAsync(bankDto);
            return NoContent();
        }

        /// <summary>
        /// Updates an existing bank with the specified identifier.
        /// </summary>
        /// <response code="204">The bank was successfully updated.</response>
        /// <response code="400">The request data is invalid.</response>
        /// <response code="404">The bank with the given ID was not found.</response>
        /// <response code="409">Bank name already exists.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Put(int id, [FromBody] BankUpdateDto bankDto)
        {
            await _service.UpdateBankAsync(id, bankDto);
            return NoContent();
        }

        /// <summary>
        /// Deletes a bank by its identifier.
        /// </summary>
        /// <response code="204">The bank was successfully deleted.</response>
        /// <response code="404">The bank with the specified ID was not found.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteBankAsync(id);
            return NoContent();
        }
    }
}
