namespace ProjectInvoices.API.Domain.Base
{
    /// <summary>
    /// Represents a standard entity in our system with unique identifier and auditing info
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Entity unique id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Entity date of creation 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Entity last modification date
        /// </summary>
        public DateTime LastModifiedDate { get; set; }
    }
}
