namespace ProjectInvoices.API.Domain
{
    /// <summary>
    /// Represents a construction material or expense 
    /// </summary>
    public class Item : BaseEntity
    {
        /// <summary>
        /// Gets or sets item name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets item unit
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets if the item is stored item or not
        /// </summary>
        public bool AffectInventory { get; set; }
    }
}
