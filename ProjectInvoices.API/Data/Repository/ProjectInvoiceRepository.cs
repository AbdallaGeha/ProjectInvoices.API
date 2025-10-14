using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TakalNew.Data;
using TaklaNew.API.Domain;
using TaklaNew.API.Domain.IRepository;

namespace TaklaNew.API.Data.Repository
{
    /// <summary>
    /// Concrete implementation of IProjectInvoiceRepository
    /// </summary>
    public class ProjectInvoiceRepository : IProjectInvoiceRepository
    {
        private readonly ApplicationDbContext _context;
        public ProjectInvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        /// <inheritdoc/>
        public async Task<ProjectInvoice?> GetProjectInvoiceWithItemsByIdAsync(int id)
        {
            return await _context.ProjectInvoices.Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == id);
        }

        /// <inheritdoc/>
        public void AddProjectInvoice(ProjectInvoice projectInvoice)
        {
            _context.ProjectInvoices.Add(projectInvoice);
        }

        /// <inheritdoc/>
        public void AddProjectInvoicePayment(ProjectInvoicePayment projectInvoicePayment)
        {
            _context.ProjectInvoicePayments.Add(projectInvoicePayment);
        }

        /// <inheritdoc/>
        public async Task<ProjectInvoicePayment?> GetProjectInvoicePaymentByIdAsync(int id)
        {
            return await _context.ProjectInvoicePayments.SingleOrDefaultAsync(x => x.Id == id);
        }

        /// <inheritdoc/>
        public async Task<List<ProjectInvoicePayment>> GetProjectInvoicePaymentsByIdsAsync(List<int> ids)
        {
            return await _context.ProjectInvoicePayments.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        /// <inheritdoc/>
        public void AddProjectInvoicePaymentGroup(ProjectInvoicePaymentGroup projectInvoicePaymentGroup)
        {
            _context.ProjectInvoicePaymentGroups.Add(projectInvoicePaymentGroup);
        }

        /// <inheritdoc/>
        public void AddCheckOutMovements(List<CheckOutMovement> checkOutMovements)
        {
            _context.CheckOutMovements.AddRange(checkOutMovements);
        }

        /// <inheritdoc/>
        public void AddCashOutMovements(List<CashOutMovement> cashOutMovements)
        {
            _context.CashOutMovements.AddRange(cashOutMovements);
        }

        /// <inheritdoc/>
        public async Task SaveAsync() 
        { 
            await _context.SaveChangesAsync(); 
        }

        /// <inheritdoc/>
        public async Task<IDbContextTransaction> BeginTransactionAsync() 
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
