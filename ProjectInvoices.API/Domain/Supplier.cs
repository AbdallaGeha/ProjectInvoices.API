using ProjectInvoices.API.Domain.Enums;

namespace ProjectInvoices.API.Domain
{
    /// <summary>
    /// Represents a supplier that provides material and services to construction companies
    /// </summary>
    public class Supplier : BaseEntity
    {
        /// <summary>
        /// Gets or sets supplier name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets supplier phone number
        /// </summary>
        public string? Phone { get; set; }
        
        /// <summary>
        /// Gets or sets supplier email
        /// </summary>
        public string? Email { get; set; }

        
        /// <summary>
        /// Gets or set supplier address
        /// </summary>
        public string? Address { get; set; }
    }
}
