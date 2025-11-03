using ProjectInvoices.API.Domain.Enums;

namespace ProjectInvoices.API.Domain
{
    /// <summary>
    /// Represents a project invoice payment group
    /// </summary>
    public class ProjectInvoicePaymentGroup : BaseEntity
    {
        /// <summary>
        /// Gets or sets payment group date
        /// </summary>
        public DateTime Date { get; set; }
    }
}
