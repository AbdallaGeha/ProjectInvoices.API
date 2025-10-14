using Microsoft.EntityFrameworkCore;
using TakalNew.Data;
using TaklaNew.API.Domain.Enums;
using TaklaNew.API.Dtos;
using TaklaNew.API.Utilities;

namespace TaklaNew.API.Services
{
    /// <summary>
    /// Concrete implementation of IProjectInvoiceQueries
    /// </summary>
    public class ProjectInvoiceQueries : IProjectInvoiceQueries
    {
        private readonly ApplicationDbContext _context;
        public ProjectInvoiceQueries(ApplicationDbContext context)
        {
            //Initialize DB context
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<List<ProjectInvoicePaymentReadyToPayDto>?> GetProjectInvoicePaymentReadyToPayAsync()
        {
            return await _context.ProjectInvoicePayments.Where(x => x.Done == false).Join(
                    _context.ProjectInvoices.Include(x => x.Project).Include(x => x.Supplier),
                    x => x.ProjectInvoiceId,
                    x => x.Id,
                    (p, i) => new ProjectInvoicePaymentReadyToPayDto
                    {
                        Id = p.Id,
                        Date = p.Date,
                        Amount = p.Amount,
                        InvoiceId = i.Id,
                        InvoiceReference = i.ReferenceNumber,
                        InvoiceDate = i.Date,
                        SupplierId = i.SupplierId,
                        Supplier = i.Supplier.Name,
                        ProjectId = i.ProjectId,
                        Project = i.Project.Name
                    }
                ).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<ProjectInvoicePaymentReadyToPayDto>?> GetProjectInvoicePaymentToPayByIdsAsync(
            List<int> paymentsIds)
        {
            return await _context.ProjectInvoicePayments.Where(x => x.Done == false && paymentsIds.Contains(x.Id)).Join(
                    _context.ProjectInvoices.Include(x => x.Project).Include(x => x.Supplier),
                    x => x.ProjectInvoiceId,
                    x => x.Id,
                    (p, i) => new ProjectInvoicePaymentReadyToPayDto
                    {
                        Id = p.Id,
                        Date = p.Date,
                        Amount = p.Amount,
                        InvoiceId = i.Id,
                        InvoiceReference = i.ReferenceNumber,
                        InvoiceDate = i.Date,
                        SupplierId = i.SupplierId,
                        Supplier = i.Supplier.Name,
                        ProjectId = i.ProjectId,
                        Project = i.Project.Name
                    }
                ).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<KeyValueDto>> GetProjectsKeyValueAsync()
        {
            return await _context.Projects
                .Select(x => new KeyValueDto { Key = x.Id.ToString(), Value = x.Name })
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<KeyValueDto>> GetSuppliersKeyValueAsync()
        {
            return await _context.Suppliers
                .Select(x => new KeyValueDto { Key = x.Id.ToString(), Value = x.Name })
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<KeyValueDto>> GetItemsKeyValueAsync()
        {
            return await _context.Items
                .Select(x => new KeyValueDto { Key = x.Id.ToString(), Value = x.Name })
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<KeyValueDto>> GetBankAccountsKeyValue()
        {
            return await _context.BankAccounts.Select(x => new KeyValueDto
            {
                Key = x.Id.ToString(),
                Value = x.AccountNumber
            }).ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<ProjectInvoiceViewDto>> GetProjectInvoiceViewAsync(int page, int pageSize, int? id,
            string? reference, int? projectId, int? supplierId, ProjectInvoiceState? state,
            DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.ProjectInvoices.Include(x => x.Items)
                .Include(x => x.Project)
                .Include(x => x.Supplier)
                .AsQueryable();

            if (id != null)
            {
                query = query.Where(x => x.Id == id);
            }

            if (reference != null)
            {
                query = query.Where(x => x.ReferenceNumber.ToLower().Contains(reference.ToLower()));
            }

            if (projectId != null)
            {
                query = query.Where(x => x.ProjectId == projectId);
            }

            if (supplierId != null)
            {
                query = query.Where(x => x.SupplierId == supplierId);
            }

            if (state != null)
            {
                query = query.Where(x => x.State == state);
            }

            if (fromDate != null && toDate != null)
            {
                query = query.Where(x => x.Date.Date >= fromDate.Value.Date && x.Date.Date <= toDate.Value.Date);
            }

            query = query.Paginate(page, pageSize);

            var result = await query.ToListAsync();

            return result.Select(x => new ProjectInvoiceViewDto { 
                Id = x.Id,
                Date = x.Date.ToString("dd-M-yyyy"),
                ReferenceNumber = x.ReferenceNumber,
                Project = x.Project.Name,
                Supplier = x.Supplier.Name,
                State = x.State.ToString(),
                Amount = x.Items.Sum(x => ((decimal)x.Quantity) * x.Price)
            }).ToList();
        }

        /// <inheritdoc/>
        public async Task<int> GetProjectInvoiceViewCountAsync(int? id,
            string? reference, int? projectId, int? supplierId, ProjectInvoiceState? state,
            DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.ProjectInvoices
                .AsQueryable();

            if (id != null)
            {
                query = query.Where(x => x.Id == id);
            }

            if (reference != null)
            {
                query = query.Where(x => x.ReferenceNumber.ToLower().Contains(reference.ToLower()));
            }

            if (projectId != null)
            {
                query = query.Where(x => x.ProjectId == projectId);
            }

            if (supplierId != null)
            {
                query = query.Where(x => x.SupplierId == supplierId);
            }

            if (state != null)
            {
                query = query.Where(x => x.State == state);
            }

            if (fromDate != null && toDate != null)
            {
                query = query.Where(x => x.Date.Date >= fromDate.Value.Date && x.Date.Date <= toDate.Value.Date);
            }

            return await query.CountAsync();
        }
    }
}
