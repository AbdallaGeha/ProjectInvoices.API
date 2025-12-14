using ProjectInvoices.API.Domain.Base;
using ProjectInvoices.API.Domain.Enums;

namespace ProjectInvoices.API.Domain
{
    /// <summary>
    /// Represents a bank account 
    /// </summary>
    public class BankAccount : BaseEntity
    {
        /// <summary>
        /// Gets or sets account number
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// Gets or sets account name
        /// </summary>
        public string AccountName { get; set; }


        /// <summary>
        /// Gets or sets bank id
        /// </summary>
        public int BankId { get; set; }

        /// <summary>
        /// Gets or sets bank
        /// </summary>
        public Bank Bank { get; set; }
    }
}
