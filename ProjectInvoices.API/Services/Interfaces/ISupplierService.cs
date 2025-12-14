using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Dtos;

namespace ProjectInvoices.API.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<SupplierUpdateGetDto> GetSupplierByIdAsync(int id);
        Task<SuppliersPaginateDto> GetSuppliersAsync(int page, int pageSize, string? search);
        Task AddSupplierAsync(SupplierCreationDto Supplier);
        Task UpdateSupplierAsync(int id, SupplierUpdateDto Supplier);
        Task DeleteSupplierAsync(int id);
    }
}
