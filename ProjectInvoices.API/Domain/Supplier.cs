using ProjectInvoices.API.Domain.Base;
using ProjectInvoices.API.Domain.Enums;

namespace ProjectInvoices.API.Domain
{
    /// <summary>
    /// Represents a supplier that provides material and services to construction companies
    /// </summary>
    public class Supplier : NamedEntity
    {
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}
