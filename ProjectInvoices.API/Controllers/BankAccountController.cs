using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Services.Interfaces;

namespace ProjectInvoices.API.Controllers
{
    /// <summary>
    /// Handles operations related to BankAccounts
    /// </summary>
    [Route("api/bankaccount")]
    [ApiController]
    [Authorize(Roles = "admin,setup entry")]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _service;

        public BankAccountController(IBankAccountService service, ILookupService lookupService)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a paginated list of BankAccounts based on the specified page number, page size,
        /// and optional search term.
        /// </summary>
        /// <response code="200">Returns the paginated list of BankAccounts.</response>
        /// <response code="400">Returned when the request parameters are invalid.</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(BankAccountsPaginateDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BankAccountsPaginateDto>> Get([FromQuery] int pageNumber, int pageSize, string? search = null)
        {
            var BankAccountsPaginateDto = await _service.GetBankAccountsAsync(pageNumber, pageSize, search);
            return Ok(BankAccountsPaginateDto);
        }


        /// <summary>
        /// Gets a BankAccount by ID.
        /// </summary>
        /// <response code="200">Returns the BankAccount data.</response>
        /// <response code="404">BankAccount not found.</response>
        /// <response code="400">Invalid Id supplied.</response>
        [ProducesResponseType(typeof(BankAccountUpdateGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BankAccountUpdateGetDto>> Get(int id)
        {
            var BankAccountUpdateGetDto = await _service.GetBankAccountByIdAsync(id);
            return Ok(BankAccountUpdateGetDto);
        }

        /// <summary>
        /// Creates a new BankAccount using the provided data.
        /// </summary>
        /// <response code="204">The BankAccount was successfully created.</response>
        /// <response code="400">The request data is invalid.</response>
        /// <response code="409">A BankAccount with the same name already exists.</response>
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Post([FromBody] BankAccountCreationDto BankAccountDto)
        {
            await _service.AddBankAccountAsync(BankAccountDto);
            return NoContent();
        }

        /// <summary>
        /// Updates an existing BankAccount with the specified identifier.
        /// </summary>
        /// <response code="204">The BankAccount was successfully updated.</response>
        /// <response code="400">The request data is invalid.</response>
        /// <response code="404">The BankAccount with the given ID was not found.</response>
        /// <response code="409">BankAccount name already exists.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Put(int id, [FromBody] BankAccountUpdateDto BankAccountDto)
        {
            await _service.UpdateBankAccountAsync(id, BankAccountDto);
            return NoContent();
        }

        /// <summary>
        /// Deletes a BankAccount by its identifier.
        /// </summary>
        /// <response code="204">The BankAccount was successfully deleted.</response>
        /// <response code="404">The BankAccount with the specified ID was not found.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteBankAccountAsync(id);
            return NoContent();
        }
    }
}
