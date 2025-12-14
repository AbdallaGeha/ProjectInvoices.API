using ProjectInvoices.API.Domain.Base;
using ProjectInvoices.API.Domain.Enums;

namespace ProjectInvoices.API.Domain
{
    /// <summary>
    /// Represents a construction project
    /// </summary>
    public class Project : NamedEntity
    {
        public ProjectState State { get; set; }
    }
}
