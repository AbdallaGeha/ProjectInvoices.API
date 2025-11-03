namespace ProjectInvoices.API.Domain.Enums
{
    /// <summary>
    /// Represents kind of transaction in our system
    /// </summary>
    public enum TransactionKind
    {
        /// <summary>
        /// Construction project invoice
        /// </summary>
        ProjectInvoice = 1,
        
        /// <summary>
        /// Construction project purchase order
        /// </summary>
        Po = 2,

        /// <summary>
        /// Construction project sub contract
        /// </summary>
        Contract = 3
    }
}
