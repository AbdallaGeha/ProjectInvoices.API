namespace ProjectInvoices.API.Dtos
{
    public class ProjectInvoiceCheckCreationDto
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int BankAccountId { get; set; }
        public string CheckNumber { get; set; }
    }
}
