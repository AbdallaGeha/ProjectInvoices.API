using System.ComponentModel.DataAnnotations;

namespace ProjectInvoices.API.Dtos
{
    public class BankUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
