using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Dtos;

namespace ProjectInvoices.API.Services.Interfaces
{
    public interface IItemService
    {
        Task<ItemUpdateGetDto> GetItemByIdAsync(int id);
        Task<ItemsPaginateDto> GetItemsAsync(int page, int pageSize, string? search);
        Task AddItemAsync(ItemCreationDto Item);
        Task UpdateItemAsync(int id, ItemUpdateDto Item);
        Task DeleteItemAsync(int id);
    }
}
