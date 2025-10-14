using TaklaNew.API.Domain;
using TaklaNew.API.Dtos;

namespace TaklaNew.API.Services
{
    /// <summary>
    /// Provides operations for managing project invoices
    /// </summary>
    public interface IProjectInvoiceService
    {
        /// <summary>
        /// Creates new project invoice
        /// </summary>
        /// <param name="projectInvoice">project invoice info</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        Task AddProjectInvoiceAsync(ProjectInvoice projectInvoice);

        /// <summary>
        /// Update an existing project invoice
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        Task UpdateProjectInvoiceAsync();

        /// <summary>
        /// Approve project invoice by changing its state and suggesting a payment
        /// </summary>
        /// <param name="invoice">project invoice info</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        Task ApproveAsync(ProjectInvoice invoice);

        /// <summary>
        /// Retrieves project invoice payment by its id
        /// </summary>
        /// <param name="id">project invoice payment id</param>
        /// <returns>
        /// a task that represents the asynchronous operation
        /// the task results contains the payment info or null if not found
        /// </returns>
        Task<ProjectInvoicePayment?> GetProjectInvoicePaymentByIdAsync(int id);

        /// <summary>
        /// Retrieves a list of project invoice payment by a list of their ids
        /// </summary>
        /// <param name="ids">a list of project invoice payment ids</param>
        /// <returns>
        /// a task that represents the asynchronous operation
        /// the task results contains the list of payments
        /// </returns>
        Task<List<ProjectInvoicePayment>> GetProjectInvoicePaymentsByIdsAsync(List<int> ids);

        /// <summary>
        /// Retrieves project invoice including all its items by its id
        /// </summary>
        /// <param name="id">project invoice id</param>
        /// <returns>
        /// a task that represents the asynchronous operation
        /// the task results contains the project invoice or null if not found
        /// </returns>
        Task<ProjectInvoice?> GetProjectInvoiceWithItemsByIdAsync(int id);
        
        /// <summary>
        /// Check if cash list info and check list info match payment info
        /// </summary>
        /// <param name="payment">project invoice payment info</param>
        /// <param name="cashList">list of cash movements</param>
        /// <param name="checkList">list of check movements</param>
        /// <returns>ture if the payment is valid, false otherwise</returns>
        bool IsValidPayment(ProjectInvoicePayment payment, List<ProjectInvoiceCashCreationDto>? cashList, List<ProjectInvoiceCheckCreationDto>? checkList);

        /// <summary>
        /// Check if cash list info and check list info match list of payments info
        /// </summary>
        /// <param name="payment">list of project invoice payments</param>
        /// <param name="cashList">list of cash movements</param>
        /// <param name="checkList">list of check movements</param>
        /// <returns>ture if group of payments are valid, false otherwise</returns>
        bool IsValidPaymentGroup(List<ProjectInvoicePayment> payments, List<ProjectInvoiceCashCreationDto>? cashList, List<ProjectInvoiceCheckCreationDto>? checkList);

        /// <summary>
        /// Pay the payment by changing its state and registering cash movements and check movements
        /// </summary>
        /// <param name="payment">project invoice payment info</param>
        /// <param name="cashList">list of cash movements</param>
        /// <param name="checkList">list of check movements</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        Task PayPaymentAsync(ProjectInvoicePayment payment, List<ProjectInvoiceCashCreationDto>? cashList, List<ProjectInvoiceCheckCreationDto>? checkList);

        /// <summary>
        /// Pay the list of payments by changing their state and registering cash movements and check movements
        /// </summary>
        /// <param name="payment">list of project invoice payments</param>
        /// <param name="cashList">list of cash movements</param>
        /// <param name="checkList">list of check movements</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        Task<bool> PayPaymentGroupAsync(List<ProjectInvoicePayment> payments, List<ProjectInvoiceCashCreationDto>? cashList, List<ProjectInvoiceCheckCreationDto>? checkList);
    }
}