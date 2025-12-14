using ProjectInvoices.API.Domain.Base;

namespace ProjectInvoices.API.Domain
{
    /// <summary>
    /// Represents a project invoice item
    /// </summary>
    public class ProjectInvoiceItem : BaseEntity
    {
        /// <summary>
        /// Gets or sets item id
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Gets or sets item 
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// Gets or sets unit
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets item quantity
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Gets or sets item price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets project invoice id
        /// </summary>
        public int ProjectInvoiceId { get; set; }
    }
}
