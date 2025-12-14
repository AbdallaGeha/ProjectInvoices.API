using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Services.Interfaces;

namespace ProjectInvoices.API.Controllers
{
    /// <summary>
    /// Handles operations related to Items
    /// </summary>
    [Route("api/item")]
    [ApiController]
    [Authorize(Roles = "admin,setup entry")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _service;
        public ItemController(IItemService service, ILookupService lookupService)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves a paginated list of Items based on the specified page number, page size,
        /// and optional search term.
        /// </summary>
        /// <response code="200">Returns the paginated list of Items.</response>
        /// <response code="400">Returned when the request parameters are invalid.</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ItemsPaginateDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ItemsPaginateDto>> Get([FromQuery] int pageNumber, int pageSize, string? search = null)
        {
            var ItemsPaginateDto = await _service.GetItemsAsync(pageNumber, pageSize, search);
            return Ok(ItemsPaginateDto);
        }


        /// <summary>
        /// Gets a Item by ID.
        /// </summary>
        /// <response code="200">Returns the Item data.</response>
        /// <response code="404">Item not found.</response>
        /// <response code="400">Invalid Id supplied.</response>
        [ProducesResponseType(typeof(ItemUpdateGetDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ItemUpdateGetDto>> Get(int id)
        {
            var ItemUpdateGetDto = await _service.GetItemByIdAsync(id);
            return Ok(ItemUpdateGetDto);
        }

        /// <summary>
        /// Creates a new Item using the provided data.
        /// </summary>
        /// <response code="204">The Item was successfully created.</response>
        /// <response code="400">The request data is invalid.</response>
        /// <response code="409">A Item with the same name already exists.</response>
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Post([FromBody] ItemCreationDto ItemDto)
        {
            await _service.AddItemAsync(ItemDto);
            return NoContent();
        }

        /// <summary>
        /// Updates an existing Item with the specified identifier.
        /// </summary>
        /// <response code="204">The Item was successfully updated.</response>
        /// <response code="400">The request data is invalid.</response>
        /// <response code="404">The Item with the given ID was not found.</response>
        /// <response code="409">Item name already exists.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Put(int id, [FromBody] ItemUpdateDto ItemDto)
        {
            await _service.UpdateItemAsync(id, ItemDto);
            return NoContent();
        }

        /// <summary>
        /// Deletes a Item by its identifier.
        /// </summary>
        /// <response code="204">The Item was successfully deleted.</response>
        /// <response code="404">The Item with the specified ID was not found.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteItemAsync(id);
            return NoContent();
        }
    }
}
