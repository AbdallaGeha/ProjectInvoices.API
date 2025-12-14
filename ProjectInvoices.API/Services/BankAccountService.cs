using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectInvoices.API.Data;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Exceptions;
using ProjectInvoices.API.Services.Interfaces;
using ProjectInvoices.API.Utilities;
using System.Data;

namespace ProjectInvoices.API.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public BankAccountService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task AddBankAccountAsync(BankAccountCreationDto bankAccount)
        {
            await EnsureAccountNameUniqueAsync(bankAccount.AccountName, null);
            await EnsureAccountNumberUniqueAsync(bankAccount.AccountNumber, null);
            var bankAccountEntity = _mapper.Map<BankAccount>(bankAccount);
            _context.BankAccounts.Add(bankAccountEntity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBankAccountAsync(int id)
        {
            var bankAccount = await GetBankAccountAsync(id);
            _context.BankAccounts.Remove(bankAccount);
            await _context.SaveChangesAsync();
        }

        public async Task<BankAccountUpdateGetDto> GetBankAccountByIdAsync(int id)
        {
            var bankAccount = await _context.BankAccounts
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);

            if (bankAccount == null)
                throw new NotFoundException("Bank Account not found.");

            return _mapper.Map<BankAccountUpdateGetDto>(bankAccount);
        }

        public async Task<BankAccountsPaginateDto> GetBankAccountsAsync(int page, int pageSize, string? search)
        {
            var query = _context.BankAccounts.Include(x => x.Bank).AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.AccountName.ToLower().Contains(search.ToLower()) ||
                x.AccountNumber.ToLower().Contains(search.ToLower()) ||
                x.Bank.Name.ToLower().Contains(search.ToLower()));
            }

            var count = await query.CountAsync();
            var bankAccounts = await query.Paginate(page, pageSize).ToListAsync();

            var bankAccountsDto = _mapper.Map<IEnumerable<BankAccountDto>>(bankAccounts);
            var bankAccountsPaginateDto = new BankAccountsPaginateDto { BankAccounts = bankAccountsDto, TotalRecords = count };

            return bankAccountsPaginateDto;
        }

        public async Task UpdateBankAccountAsync(int id, BankAccountUpdateDto bankAccount)
        {
            var bankAccountEntity = await GetBankAccountAsync(id);
            await EnsureAccountNameUniqueAsync(bankAccount.AccountName, id);
            await EnsureAccountNumberUniqueAsync(bankAccount.AccountNumber, id);
            _mapper.Map(bankAccount, bankAccountEntity);
            await _context.SaveChangesAsync();
        }

        // -----------------------------
        // Private helper methods
        // -----------------------------

        private async Task<BankAccount> GetBankAccountAsync(int id)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(id);

            if (bankAccount == null)
                throw new NotFoundException("Bank Account not found.");

            return bankAccount;
        }

        private async Task EnsureAccountNameUniqueAsync(string accountName, int? id = null)
        {
            var exists = await _context.BankAccounts
                .AnyAsync(b => b.AccountName == accountName && b.Id != id);
            
            if (exists)
                throw new DuplicateNameException("Bank account name must be unique.");
        }

        private async Task EnsureAccountNumberUniqueAsync(string accountNumber, int? id = null)
        {
            var exists = await _context.BankAccounts
                .AnyAsync(b => b.AccountName == accountNumber && b.Id != id);

            if (exists)
                throw new DuplicateNameException("Bank account number must be unique.");
        }
    }
}
