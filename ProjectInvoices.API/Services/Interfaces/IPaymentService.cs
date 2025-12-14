using Microsoft.AspNetCore.Mvc;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Dtos;

namespace ProjectInvoices.API.Services.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// Retrieves project invoice undone payments 
        /// </summary>
        Task<List<ProjectInvoicePaymentReadyToPayDto>> GetProjectInvoicePaymentReadyToPayAsync();

        /// <summary>
        /// Retrieves a list of project invoice payment by a list of their ids
        /// </summary>
        Task<List<ProjectInvoicePaymentReadyToPayDto>> GetProjectInvoicePaymentsByIdsAsync(List<int> paymentsIds);

        /// <summary>
        /// Pay the payment by changing its state and registering cash movements and check movements
        /// </summary>
        Task PayPaymentAsync(ProjectInvoicePaymentCreationDto paymentDto);

        /// <summary>
        /// Pay the list of payments by changing their state and registering cash movements and check movements
        /// </summary>
        Task PayPaymentGroupAsync(ProjectInvoiceGroupPaymentCreationDto paymentDto);
    }
}
