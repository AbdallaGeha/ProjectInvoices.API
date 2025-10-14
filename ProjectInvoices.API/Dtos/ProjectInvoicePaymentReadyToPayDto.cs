namespace TaklaNew.API.Dtos
{
    public class ProjectInvoicePaymentReadyToPayDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceReference { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int SupplierId { get; set; }
        public string Supplier { get; set; }
        public int ProjectId { get; set; }
        public string Project { get; set; }
    }
}
