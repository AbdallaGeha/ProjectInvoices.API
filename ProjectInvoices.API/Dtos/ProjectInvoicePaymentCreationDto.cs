namespace ProjectInvoices.API.Dtos
{
    public class ProjectInvoicePaymentCreationDto
    {
        public int PaymentId { get; set; }
        public List<ProjectInvoiceCheckCreationDto>? ChecksList { get; set; }
        public List<ProjectInvoiceCashCreationDto>? CashList { get; set; }
    }
}
