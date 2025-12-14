namespace ProjectInvoices.API.Domain.Base
{
    public abstract class NamedEntity : BaseEntity
    {
        public string Name { get; set; }
    }
}
