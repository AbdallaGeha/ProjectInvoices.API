using TaklaNew.API.Domain.Enums;

namespace TaklaNew.API.Domain
{
    /// <summary>
    /// Represents a sub contractor that provides construction services
    /// </summary>
    public class Contractor : BaseEntity
    {
        /// <summary>
        /// Gets or sets contractor name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets contractor phone
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets contractor email address
        /// </summary>
        public string? Email { get; set; }


        /// <summary>
        /// Gets or sets contractor address
        /// </summary>
        public string? Address { get; set; }
    }
}
