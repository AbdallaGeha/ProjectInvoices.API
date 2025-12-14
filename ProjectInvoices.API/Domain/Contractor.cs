using ProjectInvoices.API.Domain.Base;
using ProjectInvoices.API.Domain.Enums;

namespace ProjectInvoices.API.Domain
{
    /// <summary>
    /// Represents a sub contractor that provides construction services
    /// </summary>
    public class Contractor : NamedEntity
    {
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}
