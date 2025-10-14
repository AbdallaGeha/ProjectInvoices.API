using Microsoft.EntityFrameworkCore;
using TakalNew.Data;
using TaklaNew.API.Domain;
using TaklaNew.API.Domain.IRepository;
using TaklaNew.API.Utilities;

namespace TaklaNew.API.Data.Repository
{
    /// <summary>
    /// Concrete implementation of IBankAccountRepository
    /// </summary>
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public BankAccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task AddBankAccountAsync(BankAccount bankAccount)
        {
            _context.BankAccounts.Add(bankAccount);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteBankAccountAsync(BankAccount bankAccount)
        {
            _context.BankAccounts.Remove(bankAccount);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<IList<BankAccount>> GetAllBankAccountsAsync()
        {
            return await _context.BankAccounts.Include(x => x.Bank).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<BankAccount?> GetBankAccountByIdAsync(int id)
        {
            return await _context.BankAccounts.SingleOrDefaultAsync(x => x.Id == id);
        }

        /// <inheritdoc/>
        public async Task<IList<BankAccount>> GetBankAccountsAsync(int page, int pageSize, string? search)
        {
            var query = _context.BankAccounts.Include(x => x.Bank).AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.AccountName.ToLower().Contains(search.ToLower()) ||
                x.AccountNumber.ToLower().Contains(search.ToLower()) ||
                x.Bank.Name.ToLower().Contains(search.ToLower()));
            }

            query = query.Paginate(page, pageSize);

            return await query.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<int> GetTotalRecords(string? search)
        {
            var query = _context.BankAccounts.AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.AccountName.ToLower().Contains(search.ToLower()) ||
                x.AccountNumber.ToLower().Contains(search.ToLower()) ||
                x.Bank.Name.ToLower().Contains(search.ToLower()));
            }

            return await query.CountAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> IsExistingAccountNameAsync(string name)
        {
            return await _context.BankAccounts.AnyAsync(x => x.AccountName == name);
        }

        /// <inheritdoc/>
        public async Task<bool> IsExistingAccountNumberAsync(string accountNumber)
        {
            return await _context.BankAccounts.AnyAsync(x => x.AccountNumber == accountNumber);
        }

        /// <inheritdoc/>
        public async Task UpdateBankAccountAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
