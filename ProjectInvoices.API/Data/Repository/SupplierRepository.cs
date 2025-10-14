using Microsoft.EntityFrameworkCore;
using TakalNew.Data;
using TaklaNew.API.Domain;
using TaklaNew.API.Domain.IRepository;
using TaklaNew.API.Utilities;

namespace TaklaNew.API.Data.Repository
{
    /// <summary>
    /// Concrete implementation of ISupplierRepository
    /// </summary>
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ApplicationDbContext _context;

        public SupplierRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task AddSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<IList<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            return await _context.Suppliers.SingleOrDefaultAsync(x => x.Id == id);
        }

        /// <inheritdoc/>
        public async Task<IList<Supplier>> GetSuppliersAsync(int page, int pageSize, string? search)
        {
            var query = _context.Suppliers.AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()) ||
                (x.Phone != null && x.Phone.ToLower().Contains(search.ToLower())) ||
                (x.Email != null && x.Email.ToLower().Contains(search.ToLower())) ||
                (x.Address != null && x.Address.ToLower().Contains(search.ToLower()))
                );
            }

            query = query.Paginate(page, pageSize);

            return await query.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<int> GetTotalRecords(string? search)
        {
            var query = _context.Suppliers.AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
            }

            return await query.CountAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> IsExistingSupplierAsync(string name)
        {
            return await _context.Suppliers.Where(x => x.Name == name).AnyAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateSupplierAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
