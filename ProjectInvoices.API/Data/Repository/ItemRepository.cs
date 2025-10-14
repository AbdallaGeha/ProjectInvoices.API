using Microsoft.EntityFrameworkCore;
using TakalNew.Data;
using TaklaNew.API.Domain;
using TaklaNew.API.Domain.IRepository;
using TaklaNew.API.Utilities;

namespace TaklaNew.API.Data.Repository
{
    /// <summary>
    /// Concrete implementation of IItemRepository
    /// </summary>
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task AddItemAsync(Item Item)
        {
            _context.Items.Add(Item);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteItemAsync(Item Item)
        {
            _context.Items.Remove(Item);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<IList<Item>> GetAllItemsAsync()
        {
            return await _context.Items.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Item?> GetItemByIdAsync(int id)
        {
            return await _context.Items.SingleOrDefaultAsync(x => x.Id == id);
        }

        /// <inheritdoc/>
        public async Task<IList<Item>> GetItemsAsync(int page, int pageSize, string? search)
        {
            var query = _context.Items.AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()) ||
                x.Unit.ToLower().Contains(search.ToLower()));
            }

            query = query.Paginate(page, pageSize);

            return await query.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<int> GetTotalRecords(string? search)
        {
            var query = _context.Items.AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()) ||
                x.Unit.ToLower().Contains(search.ToLower()));
            }

            return await query.CountAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> IsExistingItemAsync(string name)
        {
            return await _context.Items.Where(x => x.Name == name).AnyAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateItemAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
