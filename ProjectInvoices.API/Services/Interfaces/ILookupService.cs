using Microsoft.EntityFrameworkCore;
using ProjectInvoices.API.Dtos;

namespace ProjectInvoices.API.Services.Interfaces
{
    public interface ILookupService
    {
        Task<List<KeyValueDto>> GetBanksLookup();
        Task<List<KeyValueDto>> GetProjectsLookup();
        Task<List<KeyValueDto>> GetSuppliersLookup();
        Task<List<KeyValueDto>> GetItemsLookup();
        Task<List<KeyValueDto>> GetBankAccountsLookup();
    }
}
