using System.ComponentModel.DataAnnotations;

namespace TaklaNew.API.Dtos
{
    public class BankAccountCreationDto
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
