using Microsoft.EntityFrameworkCore.Storage;
using TaklaNew.API.Domain;

namespace TaklaNew.API.Domain.IRepository
{
    /// <summary>
    /// Defines data access operations for projectInvoices
    /// </summary>
    public interface IProjectInvoiceRepository
    {
        /// <summary>
        /// Add list of cash movements to relevant DbSet
        /// </summary>
        void AddCashOutMovements(List<CashOutMovement> cashOutMovements);

        /// <summary>
        /// Add list of check movements to relevant DbSet
        /// </summary>
        void AddCheckOutMovements(List<CheckOutMovement> checkOutMovements);
        
        /// <summary>
        /// Add project invoice to relevant DbSet
        /// </summary>
        void AddProjectInvoice(ProjectInvoice projectInvoice);

        /// <summary>
        /// Add project invoice payment to relevant DbSet
        /// </summary>
        void AddProjectInvoicePayment(ProjectInvoicePayment projectInvoicePayment);

        /// <summary>
        /// Add project invoice payment group to relevant DbSet
        /// </summary>
        void AddProjectInvoicePaymentGroup(ProjectInvoicePaymentGroup projectInvoicePaymentGroup);
        
        /// <summary>
        /// Start a DB transaction
        /// </summary>
        Task<IDbContextTransaction> BeginTransactionAsync();

        /// <summary>
        /// Retrieves project invoice payment by its id
        /// </summary>
        Task<ProjectInvoicePayment?> GetProjectInvoicePaymentByIdAsync(int id);

        /// <summary>
        /// Retrieves list of project invoice payments by their ids
        /// </summary>
        Task<List<ProjectInvoicePayment>> GetProjectInvoicePaymentsByIdsAsync(List<int> ids);

        /// <summary>
        /// Retrieves project invoice with all its items by its id
        /// </summary>
        Task<ProjectInvoice?> GetProjectInvoiceWithItemsByIdAsync(int id);
        
        /// <summary>
        /// Save changes to datastore
        /// </summary>
        Task SaveAsync();
    }
}