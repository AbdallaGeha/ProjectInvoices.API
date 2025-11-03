using System.ComponentModel.DataAnnotations;

namespace ProjectInvoices.API.Dtos
{
    public class BankAccountUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string AccountNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string AccountName { get; set; }

        public int BankId { get; set; }
    }
}
