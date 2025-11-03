using ProjectInvoices.API.Domain.Enums;

namespace ProjectInvoices.API.Domain
{
    /// <summary>
    /// Represents a construction project
    /// </summary>
    public class Project : BaseEntity
    {
        /// <summary>
        /// Gets or sets project name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets project state
        /// </summary>
        public ProjectState State { get; set; }
    }
}
