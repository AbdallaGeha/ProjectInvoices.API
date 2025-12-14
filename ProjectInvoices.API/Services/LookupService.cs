using Microsoft.EntityFrameworkCore;
using ProjectInvoices.API.Data;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Domain.Base;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Services.Interfaces;

namespace ProjectInvoices.API.Services
{
    public class LookupService : ILookupService
    {
        private readonly ApplicationDbContext _context;
        public LookupService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KeyValueDto>> GetBankAccountsLookup()
        {
            return await _context.BankAccounts.Select(x => new KeyValueDto
            {
                Key = x.Id.ToString(),
                Value = x.AccountNumber
            }).ToListAsync();
        }

        public async Task<List<KeyValueDto>> GetBanksLookup()
        {
            return await GetLookup<Bank>();
        }

        public async Task<List<KeyValueDto>> GetItemsLookup()
        {
            return await GetLookup<Item>();
        }

        public async Task<List<KeyValueDto>> GetProjectsLookup()
        {
            return await GetLookup<Project>();
        }

        public async Task<List<KeyValueDto>> GetSuppliersLookup()
        {
            return await GetLookup<Supplier>();
        }

        //----------------------------------------------
        // Private helper methods
        //----------------------------------------------

        private async Task<List<KeyValueDto>> GetLookup<T>() where T : NamedEntity
        {
            return await _context.Set<T>()
                .Select(x => new KeyValueDto { Key = x.Id.ToString(), Value = x.Name })
                .ToListAsync();
        }
    }
}
