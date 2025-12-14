using ProjectInvoices.API.Domain.Base;

namespace ProjectInvoices.API.Domain
{
    /// <summary>
    /// Represents a construction material or expense 
    /// </summary>
    public class Item : NamedEntity
    {
        public string Unit { get; set; }
        public bool AffectInventory { get; set; }
    }
}
