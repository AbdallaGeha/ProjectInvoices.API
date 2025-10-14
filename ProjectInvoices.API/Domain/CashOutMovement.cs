using TaklaNew.API.Domain.Enums;

namespace TaklaNew.API.Domain
{
    /// <summary>
    /// Represents a cash movement  
    /// </summary>
    public class CashOutMovement : BaseEntity
    {
        /// <summary>
        /// Gets or sets movement date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets movement amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets for what transaction kind the movement was paid
        /// </summary>
        public TransactionKind TransactionKind { get; set; }

        /// <summary>
        /// Gets or sets paymentId
        /// </summary>
        public int PaymentId { get; set; }

        /// <summary>
        /// Gets or sets if the movement belong to one payment or a group of payments
        /// </summary>
        public bool IsGroup { get; set; }
    }
}
