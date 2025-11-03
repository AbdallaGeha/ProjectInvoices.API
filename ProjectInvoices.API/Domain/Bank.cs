using ProjectInvoices.API.Domain.Enums;

namespace ProjectInvoices.API.Domain
{
    /// <summary>
    /// Represents a bank 
    /// </summary>
    public class Bank : BaseEntity
    {
        /// <summary>
        /// Gets or sets bank name
        /// </summary>
        public string Name { get; set; }
    }
}
