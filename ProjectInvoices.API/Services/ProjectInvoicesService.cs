using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectInvoices.API.Data;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Domain.Enums;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Exceptions;
using ProjectInvoices.API.Services.Interfaces;
using ProjectInvoices.API.Utilities;

namespace ProjectInvoices.API.Services
{
    public class ProjectInvoicesService : IProjectInvoicesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectInvoicesService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddProjectInvoiceAsync(ProjectInvoiceCreationDto projectInvoiceDto)
        {
            var projectInvoice = _mapper.Map<ProjectInvoice>(projectInvoiceDto);
            _context.ProjectInvoices.Add(projectInvoice);
            await _context.SaveChangesAsync();
        }

        public async Task ApproveAsync(int id)
        {
            var projectInvoice = await GetProjectInvoiceWithItemsAsync(id);
            if (projectInvoice.State != Domain.Enums.ProjectInvoiceState.Created)
                throw new BusinessException("Project invoice state must be created before approval");

            projectInvoice.State = ProjectInvoiceState.Approved;
            
            var paymentAmount = projectInvoice.Items.Sum(x => (decimal)x.Quantity * x.Price);

            var invoicePayment = new ProjectInvoicePayment
            {
                Date = DateTime.Now,
                ProjectInvoiceId = projectInvoice.Id,
                Amount = paymentAmount,
                Done = false,
                GroupId = null,
                IsGroup = false
            };

            _context.ProjectInvoicePayments.Add(invoicePayment);

            await _context.SaveChangesAsync();
        }

        public async Task<ProjectInvoiceUpdateGetDto> GetProjectInvoiceWithItemsByIdAsync(int id)
        {
            var projectInvoice = await _context.ProjectInvoices
                .Include(x => x.Items)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);

            if (projectInvoice == null)
                throw new NotFoundException("Project invoice not found.");

            return _mapper.Map<ProjectInvoiceUpdateGetDto>(projectInvoice);
        }

        public async Task UpdateProjectInvoiceAsync(int id, ProjectInvoiceUpdateDto projectInvoiceDto)
        {
            var projectInvoice = await GetProjectInvoiceWithItemsAsync(id);
            _mapper.Map(projectInvoiceDto, projectInvoice);
            await _context.SaveChangesAsync();
        }

        public async Task<ProjectInvoiceViewResponseDto> GetProjectInvoiceView(ProjectInvoiceViewRequestDto requestDto)
        {
            ProjectInvoiceState? state = requestDto.State == null ? null : (ProjectInvoiceState)requestDto.State;

            var query = _context.ProjectInvoices
                        .AsQueryable();

            if (requestDto.Id != null)
            {
                query = query.Where(x => x.Id == requestDto.Id);
            }

            if (requestDto.Reference != null)
            {
                query = query.Where(x => x.ReferenceNumber.ToLower().Contains(requestDto.Reference.ToLower()));
            }

            if (requestDto.ProjectId != null)
            {
                query = query.Where(x => x.ProjectId == requestDto.ProjectId);
            }

            if (requestDto.SupplierId != null)
            {
                query = query.Where(x => x.SupplierId == requestDto.SupplierId);
            }

            if (state != null)
            {
                query = query.Where(x => x.State == state);
            }

            if (requestDto.FromDate != null && requestDto.ToDate != null)
            {
                query = query.Where(x => x.Date.Date >= requestDto.FromDate.Value.Date && x.Date.Date <= requestDto.ToDate.Value.Date);
            }

            var count = await query.CountAsync();

            query = query.Paginate(requestDto.Page, requestDto.PageSize);

            var result = await query.Select(x => new ProjectInvoiceViewDto
            {
                Id = x.Id,
                Date = x.Date.ToString("dd-M-yyyy"),
                ReferenceNumber = x.ReferenceNumber,
                Project = x.Project.Name,
                Supplier = x.Supplier.Name,
                State = x.State.ToString(),
                Amount = x.Items.Sum(x => ((decimal)x.Quantity) * x.Price)
            }).ToListAsync();

            return new ProjectInvoiceViewResponseDto { Data = result, TotalRecords = count };
        }

        // -----------------------------
        // Private helper methods
        // -----------------------------

        private async Task<ProjectInvoice> GetProjectInvoiceWithItemsAsync(int id)
        {
            var projectInvoice = await _context.ProjectInvoices
                .Include(x => x.Items)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (projectInvoice == null)
                throw new NotFoundException("Project Invoice not found.");

            return projectInvoice;
        }

    }
}
