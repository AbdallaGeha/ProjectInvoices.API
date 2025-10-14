using TaklaNew.API.Domain.Enums;

namespace TaklaNew.API.Domain
{
    /// <summary>
    /// Represents a project invoice
    /// </summary>
    public class ProjectInvoice : BaseEntity
    {
        /// <summary>
        /// Gets or sets invoice reference number
        /// </summary>
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets invoice date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets project id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Gets or sets project 
        /// </summary>
        public Project Project { get; set; }

        /// <summary>
        /// Gets or sets supplier id
        /// </summary>
        public int SupplierId { get; set; }

        /// <summary>
        /// Gets or sets supplier 
        /// </summary>
        public Supplier Supplier { get; set; }

        /// <summary>
        /// Gets or sets invoice state 
        /// </summary>
        public ProjectInvoiceState State { get; set; }

        /// <summary>
        /// Gets or sets list of invoice items
        /// </summary>
        public IList<ProjectInvoiceItem> Items { get; set; }
    }
}
