using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Dtos;

namespace ProjectInvoices.API.Services.Interfaces
{
    public interface IProjectInvoicesService
    {
        Task AddProjectInvoiceAsync(ProjectInvoiceCreationDto projectInvoiceDto);

        Task UpdateProjectInvoiceAsync(int id, ProjectInvoiceUpdateDto projectInvoiceDto);

        /// <summary>
        /// Approve project invoice by changing its state and suggesting a payment
        /// </summary>
        Task ApproveAsync(int id);

        /// <summary>
        /// Retrieves project invoice including all its items by its id
        /// </summary>
        Task<ProjectInvoiceUpdateGetDto> GetProjectInvoiceWithItemsByIdAsync(int id);

        /// <summary>
        /// Retrieves project invoices for reporting
        /// </summary>
        Task<ProjectInvoiceViewResponseDto> GetProjectInvoiceView(ProjectInvoiceViewRequestDto requestDto);
    }
}
