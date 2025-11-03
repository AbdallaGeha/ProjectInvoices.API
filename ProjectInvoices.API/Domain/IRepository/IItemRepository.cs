using ProjectInvoices.API.Domain;

namespace ProjectInvoices.API.Domain.IRepository
{
    /// <summary>
    /// Defines data access operations for items
    /// </summary>
    public interface IItemRepository
    {
        /// <summary>
        /// Add item to datastore
        /// </summary>
        Task AddItemAsync(Item Item);

        /// <summary>
        /// Delete item from datastore
        /// </summary>
        Task DeleteItemAsync(Item Item);

        /// <summary>
        /// Retrieves all items from datastore
        /// </summary>
        Task<IList<Item>> GetAllItemsAsync();

        /// <summary>
        /// Retrieves item from datastore by its id
        /// </summary>
        Task<Item?> GetItemByIdAsync(int id);

        /// <summary>
        /// Retrieves items from datastore by paging info and search keyword
        /// </summary>
        Task<IList<Item>> GetItemsAsync(int page, int pageSize, string? search);

        /// <summary>
        /// Retrieves items count from datastore by search keyword
        /// </summary>
        Task<int> GetTotalRecords(string? search);

        /// <summary>
        /// Check if item name exists in datastore
        /// </summary>
        Task<bool> IsExistingItemAsync(string name);

        /// <summary>
        /// Update item in datastore
        /// </summary>
        Task UpdateItemAsync();
    }
}