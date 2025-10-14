namespace TaklaNew.API.Dtos
{
    public class ProjectInvoiceGroupPaymentCreationDto
    {
        public List<int> PaymentIds { get; set; }
        public List<ProjectInvoiceCheckCreationDto>? ChecksList { get; set; }
        public List<ProjectInvoiceCashCreationDto>? CashList { get; set; }
    }
}
