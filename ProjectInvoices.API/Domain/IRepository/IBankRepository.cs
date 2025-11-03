namespace ProjectInvoices.API.Domain.IRepository
{
    /// <summary>
    /// Defines data access operations for banks
    /// </summary>
    public interface IBankRepository
    {
        /// <summary>
        /// Retrieves bank from datastore by its id
        /// </summary>
        Task<Bank?> GetBankByIdAsync(int id);

        /// <summary>
        /// Retrieves all banks from datastore
        /// </summary>
        Task<IList<Bank>> GetAllBanksAsync();

        /// <summary>
        /// Retrieves banks count from datastore by search keyword
        /// </summary>
        Task<int> GetTotalRecords(string? search);

        /// <summary>
        /// Retrieves banks from datastore by paging info and search keyword
        /// </summary>
        Task<IList<Bank>> GetBanksAsync(int page, int pageSize, string? search);

        /// <summary>
        /// Check if bank name exists in datastore
        /// </summary>
        Task<bool> IsExistingBankAsync(string name);

        /// <summary>
        /// Add bank to datastore
        /// </summary>
        Task AddBankAsync(Bank bank);

        /// <summary>
        /// Update bank in datastore
        /// </summary>
        Task UpdateBankAsync();

        /// <summary>
        /// Delete bank from datastore
        /// </summary>
        Task DeleteBankAsync(Bank bank);
    }
}
