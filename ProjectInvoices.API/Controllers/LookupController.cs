using Microsoft.AspNetCore.Mvc;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Services.Interfaces;

namespace ProjectInvoices.API.Controllers
{
    /// <summary>
    /// Handles lookup operations by providing key value pairs
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly ILookupService _service;

        public LookupController(ILookupService lookupService)
        {
            _service = lookupService;
        }

        /// <summary>
        /// Retrieves a list of projects key value pairs
        /// </summary>
        /// <response code="200">a list of suppliers</response>
        [HttpGet("projects")]
        [ProducesResponseType(typeof(List<KeyValueDto>), 200)]
        public async Task<ActionResult<List<KeyValueDto>>> GetProjects()
        {
            var projects = await _service.GetProjectsLookup();
            return Ok(projects);
        }

        /// <summary>
        /// Retrieves a list of suppliers key value pairs
        /// </summary>
        /// <response code="200">a list of suppliers</response>
        [HttpGet("suppliers")]
        [ProducesResponseType(typeof(List<KeyValueDto>), 200)]
        public async Task<ActionResult<List<KeyValueDto>>> GetSuppliers()
        {
            var suppliers = await _service.GetSuppliersLookup();
            return Ok(suppliers);
        }

        /// <summary>
        /// Retrieves a list of expenses items key value pairs
        /// </summary>
        /// <response code="200">a list of items </response>
        [HttpGet("items")]
        [ProducesResponseType(typeof(List<KeyValueDto>), 200)]
        public async Task<ActionResult<List<KeyValueDto>>> GetItems()
        {
            var items = await _service.GetItemsLookup();
            return Ok(items);
        }

        /// <summary>
        /// Retrieves a list of banks key value pairs
        /// </summary>
        /// <response code="200">a list of banks</response>
        [HttpGet("banks")]
        [ProducesResponseType(typeof(List<KeyValueDto>), 200)]
        public async Task<ActionResult<List<KeyValueDto>>> GetBanks()
        {
            var banks = await _service.GetBanksLookup();
            return Ok(banks);
        }

        /// <summary>
        /// Retrieves a list of bank accounts key value pairs
        /// </summary>
        /// <response code="200">a list of bank accounts</response>
        [HttpGet("bankaccounts")]
        [ProducesResponseType(typeof(List<KeyValueDto>), 200)]
        public async Task<ActionResult<List<KeyValueDto>>> GetBankAccounts()
        {
            var bankAccounts = await _service.GetBankAccountsLookup();
            return Ok(bankAccounts);
        }
    }
}
