using ProjectInvoices.API.Domain;

namespace ProjectInvoices.API.Domain.IRepository
{
    /// <summary>
    /// Defines data access operations for suppliers
    /// </summary>
    public interface ISupplierRepository
    {
        /// <summary>
        /// Add supplier to datastore
        /// </summary>
        Task AddSupplierAsync(Supplier supplier);

        /// <summary>
        /// Delete supplier from datastore
        /// </summary>
        Task DeleteSupplierAsync(Supplier supplier);

        /// <summary>
        /// Retrieves all suppliers from datastore
        /// </summary>
        Task<IList<Supplier>> GetAllSuppliersAsync();

        /// <summary>
        /// Retrieves supplier from datastore by its id
        /// </summary>
        Task<Supplier?> GetSupplierByIdAsync(int id);

        /// <summary>
        /// Retrieves suppliers from datastore by paging info and search keyword
        /// </summary>
        Task<IList<Supplier>> GetSuppliersAsync(int page, int pageSize, string? search);

        /// <summary>
        /// Retrieves suppliers count from datastore by search keyword
        /// </summary>
        Task<int> GetTotalRecords(string? search);

        /// <summary>
        /// Check if supplier name exists in datastore
        /// </summary>
        Task<bool> IsExistingSupplierAsync(string name);
        
        /// <summary>
        /// Update supplier in datastore
        /// </summary>
        Task UpdateSupplierAsync();
    }
}