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
    /// Handles operations related to suppliers
    /// </summary>
    [Route("api/supplier")]
    [ApiController]
    [Authorize(Roles = "admin,setup entry")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierRepository _repository;
        private readonly IMapper _mapper;

        public SupplierController(ISupplierRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all suppliers
        /// </summary>
        /// <returns>List of supplier dto objects</returns>
        /// <response code="200">Returns the List of supplier dto objects</response>
        /// 
        [HttpGet("all")]
        public async Task<ActionResult<List<SupplierDto>>> GetAll()
        {
            //Get all suppliers from DB
            var suppliers = await _repository.GetAllSuppliersAsync();
            var suppliersDto = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);

            return Ok(suppliersDto);
        }

        /// <summary>
        /// Retrieves suppliers after pagination and searching
        /// </summary>
        /// <param name="pageNumber">page number</param>
        /// <param name="pageSize">page size</param>
        /// <param name="search">search keyword</param>
        /// <returns>A dto object contains list of suppliers and total number of suppliers
        /// to be used for pagination
        /// </returns>
        /// <response code="200">Returns a dto object contains list of suppliers and total number of suppliers</response>
        /// 
        [HttpGet("search")]
        public async Task<ActionResult<SuppliersPaginateDto>> Get([FromQuery] int pageNumber, int pageSize, string? search = null)
        {
            //Get total number of suppliers in DB (for pagination)
            var totalRecords = await _repository.GetTotalRecords(search);
            
            //Get suppliers by page and search keyword
            var suppliers = await _repository.GetSuppliersAsync(pageNumber, pageSize, search);

            var suppliersDto = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
            var suppliersPaginateDto = new SuppliersPaginateDto { Suppliers = suppliersDto, TotalRecords = totalRecords };

            return Ok(suppliersPaginateDto);
        }

        /// <summary>
        /// Retrieves a supplier by its id
        /// </summary>
        /// <param name="id">supplier id</param>
        /// <returns>the matching supplier dto object</returns>
        /// <response code="404">supplier not found</response>
        /// <response code="200">returns the matching supplier dto object</response>
        /// 
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SupplierUpdateGetDto>> Get(int id)
        {
            //Get supplier from DB by its id
            var supplier = await _repository.GetSupplierByIdAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            var supplierUpdateGetDto = _mapper.Map<SupplierUpdateGetDto>(supplier);
            
            return Ok(supplierUpdateGetDto);
        }

        /// <summary>
        /// Create a new supplier
        /// </summary>
        /// <param name="supplierDto">supplier info</param>
        /// <returns>no content if supplier created successfully</returns>
        /// <response code="400">supplier info are not valid or supplier name already exists</response>
        /// <response code="204">supplier created successfully</response>
        /// 
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SupplierCreationDto supplierDto)
        {
            //Check if new supplier name exists in DB
            var isExistingSupplier = await _repository.IsExistingSupplierAsync(supplierDto.Name);
            if (isExistingSupplier)
            {
                return BadRequest("The Supplier name is already in use");
            }

            var supplier = _mapper.Map<Supplier>(supplierDto);
            
            //Add new supplier to DB
            await _repository.AddSupplierAsync(supplier);
            
            return NoContent();
        }

        /// <summary>
        /// Update a supplier
        /// </summary>
        /// <param name="id">supplier id</param>
        /// <param name="supplierDto">supplier info</param>
        /// <returns>no content if supplier updated successfully</returns>
        /// <response code="404">supplier not found</response>
        /// <response code="400">supplier info are not valid or supplier name already in use</response>
        /// <response code="204">supplier updated successfully</response>
        /// 
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] SupplierUpdateDto supplierDto)
        {
            //Get supplier from DB by its id
            var supplier = await _repository.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            if (!supplierDto.Name.Equals(supplier.Name))
            {
                //Check if the supplier to update new name exists in DB
                var isExistingSupplier = await _repository.IsExistingSupplierAsync(supplierDto.Name);
                if (isExistingSupplier)
                {
                    return BadRequest("The Supplier name is already in use");
                }
            }

            _mapper.Map(supplierDto, supplier);

            //Update supplier in DB
            await _repository.UpdateSupplierAsync();
            
            return NoContent();
        }

        /// <summary>
        /// Delete a supplier
        /// </summary>
        /// <param name="id">supplier id</param>
        /// <returns>no content if supplier deleted successfully</returns>
        /// <response code="404">supplier not found</response>
        /// <response code="204">supplier deleted successfully</response>
        /// 
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            //Get supplier from DB by its id
            var supplier = await _repository.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            
            //Delete supplier from DB
            await _repository.DeleteSupplierAsync(supplier);
            
            return NoContent();
        }
    }
}
