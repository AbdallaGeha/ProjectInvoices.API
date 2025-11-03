using ProjectInvoices.API.Domain;

namespace ProjectInvoices.API.Domain.IRepository
{
    /// <summary>
    /// Defines data access operations for bank accounts
    /// </summary>
    public interface IBankAccountRepository
    {
        /// <summary>
        /// Add bank account to datastore
        /// </summary>
        Task AddBankAccountAsync(BankAccount bankAccount);

        /// <summary>
        /// Delete bank account from datastore
        /// </summary>
        Task DeleteBankAccountAsync(BankAccount bankAccount);

        /// <summary>
        /// Retrieves all bank accounts from datastore
        /// </summary>
        Task<IList<BankAccount>> GetAllBankAccountsAsync();

        /// <summary>
        /// Retrieves bank account from datastore by its id
        /// </summary>
        Task<BankAccount?> GetBankAccountByIdAsync(int id);

        /// <summary>
        /// Retrieves bank accounts from datastore by paging info and search keyword
        /// </summary>
        Task<IList<BankAccount>> GetBankAccountsAsync(int page, int pageSize, string? search);

        /// <summary>
        /// Retrieves bank accounts count from datastore by search keyword
        /// </summary>
        Task<int> GetTotalRecords(string? search);

        /// <summary>
        /// Check if account name exists in datastore
        /// </summary>
        Task<bool> IsExistingAccountNameAsync(string name);

        /// <summary>
        /// Check if account number exists in datastore
        /// </summary>
        Task<bool> IsExistingAccountNumberAsync(string accountNumber);

        /// <summary>
        /// Update bank account in datastore
        /// </summary>
        Task UpdateBankAccountAsync();
    }
}