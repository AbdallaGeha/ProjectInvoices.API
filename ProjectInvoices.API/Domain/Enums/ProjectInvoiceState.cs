namespace ProjectInvoices.API.Domain.Enums
{
    /// <summary>
    /// Represents the current state of a project invoice
    /// </summary>
    public enum ProjectInvoiceState
    {
        /// <summary>
        /// The project invoice is created and ready for approval
        /// </summary>
        Created = 1,
        
        /// <summary>
        /// the project invoice is approved and ready for payments
        /// </summary>
        Approved = 2
    }
}
