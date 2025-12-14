using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Dtos;

namespace ProjectInvoices.API.Services.Interfaces
{
    public interface IBankAccountService
    {
        Task<BankAccountUpdateGetDto> GetBankAccountByIdAsync(int id);
        Task<BankAccountsPaginateDto> GetBankAccountsAsync(int page, int pageSize, string? search);
        Task AddBankAccountAsync(BankAccountCreationDto bankAccount);
        Task UpdateBankAccountAsync(int id, BankAccountUpdateDto bankAccount);
        Task DeleteBankAccountAsync(int id);
    }
}
