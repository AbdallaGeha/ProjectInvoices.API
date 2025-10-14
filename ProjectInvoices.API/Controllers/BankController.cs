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
    /// Handles operations related to banks
    /// </summary>
    [Route("api/bank")]
    [ApiController]
    [Authorize(Roles = "admin,setup entry")]
    public class BankController : ControllerBase
    {
        private readonly IBankRepository _repository;
        private readonly IMapper _mapper;

        public BankController(IBankRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all banks
        /// </summary>
        /// <returns>List of bank dto objects</returns>
        /// <response code="200">Returns the List of bank dto objects</response>
        [HttpGet("all")]
        public async Task<ActionResult<List<BankDto>>> GetAll()
        {
            //Get all banks from DB
            var banks = await _repository.GetAllBanksAsync();
            var banksDto = _mapper.Map<IEnumerable<BankDto>>(banks);

            return Ok(banksDto);
        }

        /// <summary>
        /// Retrieves banks after pagination and searching
        /// </summary>
        /// <param name="pageNumber">page number</param>
        /// <param name="pageSize">page size</param>
        /// <param name="search">search keyword</param>
        /// <returns>A dto object contains list of banks and total number of banks
        /// to be used for pagination
        /// </returns>
        /// <response code="200">Returns a dto object contains list of banks and total number of banks</response>
        /// 
        [HttpGet("search")]
        public async Task<ActionResult<BanksPaginateDto>> Get([FromQuery] int pageNumber, int pageSize, string? search = null)
        {
            //Get total number of banks in DB (for pagination)
            var totalRecords = await _repository.GetTotalRecords(search);

            //Get banks by page and search keyword
            var banks = await _repository.GetBanksAsync(pageNumber, pageSize, search);

            var banksDto = _mapper.Map<IEnumerable<BankDto>>(banks);
            var banksPaginateDto = new BanksPaginateDto { Banks = banksDto, TotalRecords = totalRecords };

            return Ok(banksPaginateDto);
        }


        /// <summary>
        /// Retrieves a bank by its id
        /// </summary>
        /// <param name="id">bank id</param>
        /// <returns>the matching bank dto object</returns>
        /// <response code="404">bank not found</response>
        /// <response code="200">returns the matching bank dto object</response>
        ///
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BankUpdateGetDto>> Get(int id)
        {
            //Get bank from DB by its id
            var bank = await _repository.GetBankByIdAsync(id);

            if (bank == null)
            {
                return NotFound();
            }

            var bankUpdateGetDto = _mapper.Map<BankUpdateGetDto>(bank);
            return Ok(bankUpdateGetDto);
        }

        /// <summary>
        /// Create a new bank
        /// </summary>
        /// <param name="bankDto">bank info</param>
        /// <returns>no content if bank created successfully</returns>
        /// <response code="400">bank info are not valid or bank name already exists</response>
        /// <response code="204">bank created successfully</response>
        ///
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BankCreationDto bankDto)
        {
            //Check if new bank name exists in DB
            var isExistingBank = await _repository.IsExistingBankAsync(bankDto.Name);
            if (isExistingBank)
            {
                return BadRequest("The Bank name is already in use");
            }

            var bank = _mapper.Map<Bank>(bankDto);

            //Add new bank to DB
            await _repository.AddBankAsync(bank);
            
            return NoContent();
        }

        /// <summary>
        /// Update a bank
        /// </summary>
        /// <param name="id">bank id</param>
        /// <param name="bankDto">bank info</param>
        /// <returns>no content if bank updated successfully</returns>
        /// <response code="404">bank not found</response>
        /// <response code="400">bank info are not valid or bank name already in use</response>
        /// <response code="204">bank updated successfully</response>
        ///
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] BankUpdateDto bankDto)
        {
            //Get bank from DB by its id
            var bank = await _repository.GetBankByIdAsync(id);
            if (bank == null)
            {
                return NotFound();
            }

            if (!bankDto.Name.Equals(bank.Name))
            {
                //Check if the bank to update new name exists in DB
                var isExistingBank = await _repository.IsExistingBankAsync(bankDto.Name);
                if (isExistingBank)
                {
                    return BadRequest("The Bank name is already in use");
                }
            }

            _mapper.Map(bankDto, bank);

            //Update bank in DB
            await _repository.UpdateBankAsync();
            return NoContent();
        }

        /// <summary>
        /// Delete a bank
        /// </summary>
        /// <param name="id">bank id</param>
        /// <returns>no content if bank deleted successfully</returns>
        /// <response code="404">bank not found</response>
        /// <response code="204">bank deleted successfully</response>
        ///
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            //Get bank from DB by its id
            var bank = await _repository.GetBankByIdAsync(id);
            if (bank == null)
            {
                return NotFound();
            }

            //Delete bank from DB
            await _repository.DeleteBankAsync(bank);
            return NoContent();
        }

        /// <summary>
        /// Retrieves a list of banks as a list of keyvalue pairs
        /// </summary>
        /// <returns>a list of banks as a list of keyvalue pairs</returns>
        /// <response code="200">a list of banks as a list of keyvalue pairs</response>
        ///
        [HttpGet("bankskeyvalue")]
        public async Task<ActionResult<List<KeyValueDto>>> GetBanksKeyValue()
        {
            //Get all banks from DB
            var banks = await _repository.GetAllBanksAsync();
            var result = banks.Select(x => new KeyValueDto { Key = x.Id.ToString(), Value = x.Name.ToString() });
            return Ok(result);
        }
    }
}
