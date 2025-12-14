using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Dtos;

namespace ProjectInvoices.API.Services.Interfaces
{
    public interface IBankService
    {
        Task<BankUpdateGetDto> GetBankByIdAsync(int id);
        Task<BanksPaginateDto> GetBanksAsync(int page, int pageSize, string? search);
        Task AddBankAsync(BankCreationDto bank);
        Task UpdateBankAsync(int id, BankUpdateDto bank);
        Task DeleteBankAsync(int id);
    }
}
