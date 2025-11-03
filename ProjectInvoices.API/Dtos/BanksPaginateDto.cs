namespace ProjectInvoices.API.Dtos
{
    public class BanksPaginateDto
    {
        public IEnumerable<BankDto> Banks { get; set; } = new List<BankDto>();

        public int TotalRecords { get; set; }
    }
}
