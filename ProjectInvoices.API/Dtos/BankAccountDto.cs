using System.ComponentModel.DataAnnotations;

namespace TaklaNew.API.Dtos
{
    public class BankAccountDto
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Bank { get; set; }
    }
}
