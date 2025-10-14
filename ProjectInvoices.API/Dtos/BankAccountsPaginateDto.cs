namespace TaklaNew.API.Dtos
{
    public class BankAccountsPaginateDto
    {
        public IEnumerable<BankAccountDto> BankAccounts { get; set; } = new List<BankAccountDto>();

        public int TotalRecords { get; set; }
    }
}
