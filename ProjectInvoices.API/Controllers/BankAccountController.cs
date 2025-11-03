using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Domain.IRepository;
using ProjectInvoices.API.Dtos;

namespace ProjectInvoices.API.Controllers
{
    /// <summary>
    /// Handles operations related to bank accounts
    /// </summary>
    [Route("api/bankaccount")]
    [ApiController]
    [Authorize(Roles = "admin,setup entry")]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountRepository _repository;
        private readonly IMapper _mapper;
        public BankAccountController(IBankAccountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all bank accounts
        /// </summary>
        /// <returns>List of bank account dto objects</returns>
        /// <response code="200">Returns the List of bank account dto objects</response>
        ///
        [HttpGet("all")]
        public async Task<ActionResult<List<BankAccountDto>>> GetAll()
        {
            //Get all bank accounts from DB
            var bankAccounts = await _repository.GetAllBankAccountsAsync();
            var bankAccountsDto = _mapper.Map<IEnumerable<BankAccountDto>>(bankAccounts);

            return Ok(bankAccountsDto);
        }

        /// <summary>
        /// Retrieves bank accounts after pagination and searching
        /// </summary>
        /// <param name="pageNumber">page number</param>
        /// <param name="pageSize">page size</param>
        /// <param name="search">search keyword</param>
        /// <returns>A dto object contains list of bank accounts and total number of bank accounts
        /// to be used for pagination
        /// </returns>
        /// <response code="200">Returns a dto object contains list of bank accounts and total number of bank accounts</response>
        ///
        [HttpGet("search")]
        public async Task<ActionResult<BankAccountsPaginateDto>> Get([FromQuery] int pageNumber, int pageSize, string? search = null)
        {
            //Get total number of bank accounts in DB (for pagination)
            var totalRecords = await _repository.GetTotalRecords(search);

            //Get bank accounts by page and search keyword
            var bankAccounts = await _repository.GetBankAccountsAsync(pageNumber, pageSize, search);

            var bankAccountsDto = _mapper.Map<IEnumerable<BankAccountDto>>(bankAccounts);
            var bankAccountsPaginateDto = new BankAccountsPaginateDto { BankAccounts = bankAccountsDto, TotalRecords = totalRecords };

            return Ok(bankAccountsPaginateDto);
        }

        /// <summary>
        /// Retrieves a bank account by its id
        /// </summary>
        /// <param name="id">bank account id</param>
        /// <returns>the matching bank account dto object</returns>
        /// <response code="404">bank account not found</response>
        /// <response code="200">returns the matching bank account dto object</response>
        ///
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BankAccountUpdateGetDto>> Get(int id)
        {
            //Get bank account from DB by its id
            var bankAccount = await _repository.GetBankAccountByIdAsync(id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            var bankAccountUpdateGetDto = _mapper.Map<BankAccountUpdateGetDto>(bankAccount);
            return Ok(bankAccountUpdateGetDto);
        }

        /// <summary>
        /// Create a new bank account
        /// </summary>
        /// <param name="bank accountDto">bank account info</param>
        /// <returns>no content if bank account created successfully</returns>
        /// <response code="400">bank account info are not valid or bank account name already exists</response>
        /// <response code="204">bank account created successfully</response>
        ///
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BankAccountCreationDto bankAccountDto)
        {
            //Check if new bank account name exists in DB
            var isExistingAccountName = await _repository.IsExistingAccountNameAsync(bankAccountDto.AccountName);
            if (isExistingAccountName)
            {
                return BadRequest("Account name is already in use");
            }

            var isExistingAccountNumber = await _repository.IsExistingAccountNumberAsync(bankAccountDto.AccountNumber);
            if (isExistingAccountNumber)
            {
                return BadRequest("Account number is already in use");
            }

            var bankAccount = _mapper.Map<BankAccount>(bankAccountDto);

            //Add new bank account to DB
            await _repository.AddBankAccountAsync(bankAccount);
            
            return NoContent();
        }

        /// <summary>
        /// Update a bank account
        /// </summary>
        /// <param name="id">bank account id</param>
        /// <param name="bank accountDto">bank account info</param>
        /// <returns>no content if bank account updated successfully</returns>
        /// <response code="404">bank account not found</response>
        /// <response code="400">bank account info are not valid or bank account name already in use</response>
        /// <response code="204">bank account updated successfully</response>
        ///
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] BankAccountUpdateDto bankAccountDto)
        {
            //Get bank account from DB by its id
            var bankAccount = await _repository.GetBankAccountByIdAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            if (!bankAccountDto.AccountName.Equals(bankAccount.AccountName))
            {
                //Check if the bank account to update new name exists in DB
                var isExistingAccountName = await _repository.IsExistingAccountNameAsync(bankAccountDto.AccountName);
                if (isExistingAccountName)
                {
                    return BadRequest("Account name is already in use");
                }
            }

            var isExistingAccountNumber = await _repository.IsExistingAccountNumberAsync(bankAccountDto.AccountNumber);
            if (isExistingAccountNumber && !bankAccountDto.AccountNumber.Equals(bankAccount.AccountNumber))
            {
                return BadRequest("Account number is already in use");
            }

            _mapper.Map(bankAccountDto, bankAccount);

            //Update bank account in DB
            await _repository.UpdateBankAccountAsync();
            
            return NoContent();
        }

        /// <summary>
        /// Delete a bank account
        /// </summary>
        /// <param name="id">bank account id</param>
        /// <returns>no content if bank account deleted successfully</returns>
        /// <response code="404">bank account not found</response>
        /// <response code="204">bank account deleted successfully</response>
        ///
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            //Get bank account from DB by its id
            var bankAccount = await _repository.GetBankAccountByIdAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            //Delete bank account from DB
            await _repository.DeleteBankAccountAsync(bankAccount);
            
            return NoContent();
        }
    }
}
