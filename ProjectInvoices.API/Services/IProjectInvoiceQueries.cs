using ProjectInvoices.API.Domain.Enums;
using ProjectInvoices.API.Dtos;

namespace ProjectInvoices.API.Services
{
    /// <summary>
    /// Provides query operations for project invoices
    /// </summary>
    public interface IProjectInvoiceQueries
    {
        /// <summary>
        /// Retrieves list of all suggested payments detailed info 
        /// </summary>
        /// <returns>
        /// a task that represents the asynchronous operation
        /// the task results contains list of all suggested payments detailed info
        /// </returns>
        Task<List<ProjectInvoicePaymentReadyToPayDto>?> GetProjectInvoicePaymentReadyToPayAsync();

        /// <summary>
        /// Retrieves list of suggested payments detailed info by their ids
        /// </summary>
        /// <returns>
        /// a task that represents the asynchronous operation
        /// the task results contains list of suggested payments detailed info by their ids
        /// </returns>
        Task<List<ProjectInvoicePaymentReadyToPayDto>?> GetProjectInvoicePaymentToPayByIdsAsync(List<int> paymentsIds);

        /// <summary>
        /// Retrieves list of projects (id and name)
        /// </summary>
        /// <returns>
        /// a task that represents the asynchronous operation
        /// the task results contains list of projects (id and name)
        /// </returns>
        Task<List<KeyValueDto>> GetProjectsKeyValueAsync();

        /// <summary>
        /// Retrieves list of suppliers (id and name)
        /// </summary>
        /// <returns>
        /// a task that represents the asynchronous operation
        /// the task results contains list of suppliers (id and name)
        /// </returns>        
        Task<List<KeyValueDto>> GetSuppliersKeyValueAsync();

        /// <summary>
        /// Retrieves list of items (id and name)
        /// </summary>
        /// <returns>
        /// a task that represents the asynchronous operation
        /// the task results contains list of items (id and name)
        /// </returns>        
        Task<List<KeyValueDto>> GetItemsKeyValueAsync();

        /// <summary>
        /// Retrieves list of bank accounts (id and name)
        /// </summary>
        /// <returns>
        /// a task that represents the asynchronous operation
        /// the task results contains list of bank accounts (id and name)
        /// </returns>                
        Task<List<KeyValueDto>> GetBankAccountsKeyValue();

        /// <summary>
        /// Retrieves list of project invoices detailed info by provided pagination and criteria
        /// </summary>
        /// <param name="page">page number</param>
        /// <param name="pageSize">page size</param>
        /// <param name="id">invoice id</param>
        /// <param name="reference">invoice reference</param>
        /// <param name="projectId">project id</param>
        /// <param name="supplierId">supplier id</param>
        /// <param name="state">invoice state</param>
        /// <param name="fromDate">from date</param>
        /// <param name="toDate">to date</param>
        /// <returns>
        /// a task that represents the asynchronous operation
        /// the task results contains list of project invoices detailed info
        /// </returns>
        Task<List<ProjectInvoiceViewDto>> GetProjectInvoiceViewAsync(int page, int pageSize, int? id,
            string? reference, int? projectId, int? supplierId, ProjectInvoiceState? state,
            DateTime? fromDate, DateTime? toDate);

        /// <summary>
        /// Retrieves count of of project invoices by provided criteria
        /// </summary>
        /// <param name="id">invoice id</param>
        /// <param name="reference">invoice reference</param>
        /// <param name="projectId">project id</param>
        /// <param name="supplierId">supplier id</param>
        /// <param name="state">invoice state</param>
        /// <param name="fromDate">from date</param>
        /// <param name="toDate">to date</param>
        /// <returns>
        /// a task that represents the asynchronous operation
        /// the task results contains count of of project invoices
        /// </returns>
        Task<int> GetProjectInvoiceViewCountAsync(int? id,
            string? reference, int? projectId, int? supplierId, ProjectInvoiceState? state,
            DateTime? fromDate, DateTime? toDate);
    }
}