using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectInvoices.API.Data;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Exceptions;
using ProjectInvoices.API.Services.Base;
using ProjectInvoices.API.Services.Interfaces;
using ProjectInvoices.API.Utilities;

namespace ProjectInvoices.API.Services
{
    public class ItemService : NamedEntityService<Item>, IItemService
    {
        private readonly IMapper _mapper;
        public ItemService(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }
        public async Task AddItemAsync(ItemCreationDto Item)
        {
            await EnsureNameUniqueAsync(Item.Name, null);
            var ItemEntity = _mapper.Map<Item>(Item);
            _context.Items.Add(ItemEntity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int id)
        {
            var Item = await GetAsync(id);
            _context.Items.Remove(Item);
            await _context.SaveChangesAsync();
        }

        public async Task<ItemUpdateGetDto> GetItemByIdAsync(int id)
        {
            var Item = await _context.Items
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);

            if (Item == null)
                throw new NotFoundException("Item not found.");

            return _mapper.Map<ItemUpdateGetDto>(Item);
        }

        public async Task<ItemsPaginateDto> GetItemsAsync(int page, int pageSize, string? search)
        {
            var query = _context.Items.AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()) ||
                x.Unit.ToLower().Contains(search.ToLower()));
            }

            var count = await query.CountAsync();
            var Items = await query.Paginate(page, pageSize).ToListAsync();

            var ItemsDto = _mapper.Map<IEnumerable<ItemDto>>(Items);
            var ItemsPaginateDto = new ItemsPaginateDto { Items = ItemsDto, TotalRecords = count };

            return ItemsPaginateDto;
        }

        public async Task UpdateItemAsync(int id, ItemUpdateDto Item)
        {
            var ItemEntity = await GetAsync(id);
            await EnsureNameUniqueAsync(Item.Name, id);
            _mapper.Map(Item, ItemEntity);
            await _context.SaveChangesAsync();
        }
    }
}
