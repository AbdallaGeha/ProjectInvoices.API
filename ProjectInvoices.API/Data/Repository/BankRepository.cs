using Microsoft.EntityFrameworkCore;
using TakalNew.Data;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Domain.IRepository;
using ProjectInvoices.API.Utilities;

namespace ProjectInvoices.API.Data.Repository
{
    /// <summary>
    /// Concrete implementation of IBankRepository
    /// </summary>
    public class BankRepository : IBankRepository
    {
        private readonly ApplicationDbContext _context;

        public BankRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task AddBankAsync(Bank bank)
        {
            _context.Banks.Add(bank);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteBankAsync(Bank bank)
        {
            _context.Banks.Remove(bank);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<IList<Bank>> GetAllBanksAsync()
        {
            return await _context.Banks.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Bank?> GetBankByIdAsync(int id)
        {
            return await _context.Banks.SingleOrDefaultAsync(x => x.Id == id);
        }

        /// <inheritdoc/>
        public async Task<IList<Bank>> GetBanksAsync(int page, int pageSize, string? search)
        {
            var query = _context.Banks.AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
            }

            query = query.Paginate(page, pageSize);

            return await query.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<int> GetTotalRecords(string? search)
        {
            var query = _context.Banks.AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
            }

            return await query.CountAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> IsExistingBankAsync(string name)
        {
            return await _context.Banks.Where(x => x.Name == name).AnyAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateBankAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
