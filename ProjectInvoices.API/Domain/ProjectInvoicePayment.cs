using TaklaNew.API.Domain.Enums;

namespace TaklaNew.API.Domain
{
    /// <summary>
    /// Represents a project invoice payment
    /// </summary>
    public class ProjectInvoicePayment : BaseEntity
    {
        /// <summary>
        /// Gets or sets payment date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets payment amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets project invoice id
        /// </summary>
        public int ProjectInvoiceId { get; set; }

        /// <summary>
        /// Gets or sets if the payment paid as a group of payments
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Gets or sets payment group id if the payment paid as a group of payments
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// Gets or sets payment status, paid or not
        /// </summary>
        public bool Done { get; set; }
    }
}
