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
    /// Handles operations related to items
    /// </summary>
    [Route("api/item")]
    [ApiController]
    [Authorize(Roles = "admin,setup entry")]
    public class itemController : ControllerBase
    {
        private readonly IItemRepository _repository;
        private readonly IMapper _mapper;

        public itemController(IItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all items
        /// </summary>
        /// <returns>List of item dto objects</returns>
        /// <response code="200">Returns the List of item dto objects</response>
        ///
        [HttpGet("all")]
        public async Task<ActionResult<List<ItemDto>>> GetAll()
        {
            //Get all items from DB
            var items = await _repository.GetAllItemsAsync();
            var itemsDto = _mapper.Map<IEnumerable<ItemDto>>(items);

            return Ok(itemsDto);
        }

        /// <summary>
        /// Retrieves items after pagination and searching
        /// </summary>
        /// <param name="pageNumber">page number</param>
        /// <param name="pageSize">page size</param>
        /// <param name="search">search keyword</param>
        /// <returns>A dto object contains list of items and total number of items
        /// to be used for pagination
        /// </returns>
        /// <response code="200">Returns a dto object contains list of items and total number of items</response>
        /// 
        [HttpGet("search")]
        public async Task<ActionResult<ItemsPaginateDto>> Get([FromQuery] int pageNumber, int pageSize, string? search = null)
        {
            //Get total number of items in DB (for pagination)
            var totalRecords = await _repository.GetTotalRecords(search);

            //Get items by page and search keyword
            var items = await _repository.GetItemsAsync(pageNumber, pageSize, search);

            var itemsDto = _mapper.Map<IEnumerable<ItemDto>>(items);
            var itemsPaginateDto = new ItemsPaginateDto { Items = itemsDto, TotalRecords = totalRecords };

            return Ok(itemsPaginateDto);
        }

        /// <summary>
        /// Retrieves a item by its id
        /// </summary>
        /// <param name="id">item id</param>
        /// <returns>the matching item dto object</returns>
        /// <response code="404">item not found</response>
        /// <response code="200">returns the matching item dto object</response>
        /// 
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ItemUpdateGetDto>> Get(int id)
        {
            //Get item from DB by its id
            var item = await _repository.GetItemByIdAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            var itemUpdateGetDto = _mapper.Map<ItemUpdateGetDto>(item);
            return Ok(itemUpdateGetDto);
        }

        /// <summary>
        /// Create a new item
        /// </summary>
        /// <param name="itemDto">item info</param>
        /// <returns>no content if item created successfully</returns>
        /// <response code="400">item info are not valid or item name already exists</response>
        /// <response code="204">item created successfully</response>
        ///
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ItemCreationDto itemDto)
        {
            //Check if new item name exists in DB
            var isExistingitem = await _repository.IsExistingItemAsync(itemDto.Name);
            if (isExistingitem)
            {
                return BadRequest("Item name is already in use");
            }

            var item = _mapper.Map<Item>(itemDto);

            //Add new item to DB
            await _repository.AddItemAsync(item);
            
            return NoContent();
        }

        /// <summary>
        /// Update a item
        /// </summary>
        /// <param name="id">item id</param>
        /// <param name="itemDto">item info</param>
        /// <returns>no content if item updated successfully</returns>
        /// <response code="404">item not found</response>
        /// <response code="400">item info are not valid or item name already in use</response>
        /// <response code="204">item updated successfully</response>
        ///
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ItemUpdateDto itemDto)
        {
            //Get item from DB by its id
            var item = await _repository.GetItemByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            if (!itemDto.Name.Equals(item.Name))
            {
                //Check if the item to update new name exists in DB
                var isExistingitem = await _repository.IsExistingItemAsync(itemDto.Name);
                if (isExistingitem)
                {
                    return BadRequest("Item name is already in use");
                }
            }

            _mapper.Map(itemDto, item);

            //Update item in DB
            await _repository.UpdateItemAsync();
            return NoContent();
        }

        /// <summary>
        /// Delete a item
        /// </summary>
        /// <param name="id">item id</param>
        /// <returns>no content if item deleted successfully</returns>
        /// <response code="404">item not found</response>
        /// <response code="204">item deleted successfully</response>
        ///
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            //Get item from DB by its id
            var item = await _repository.GetItemByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            //Delete item from DB
            await _repository.DeleteItemAsync(item);
            
            return NoContent();
        }

        /// <summary>
        /// Retrieves a list of items as a list of keyvalue pairs
        /// </summary>
        /// <returns>a list of items as a list of keyvalue pairs</returns>
        /// <response code="200">a list of items as a list of keyvalue pairs</response>
        ///
        [HttpGet("itemskeyvalue")]
        public async Task<ActionResult<List<KeyValueDto>>> GetitemsKeyValue()
        {
            //Get all items from DB
            var items = await _repository.GetAllItemsAsync();
            var result = items.Select(x => new KeyValueDto { Key = x.Id.ToString(), Value = x.Name.ToString() });
            return Ok(result);
        }
    }
}
