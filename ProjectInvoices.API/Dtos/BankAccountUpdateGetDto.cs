using System.ComponentModel.DataAnnotations;

namespace TaklaNew.API.Dtos
{
    public class BankAccountUpdateGetDto
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public int BankId { get; set; }
    }
}
