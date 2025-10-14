using System.ComponentModel.DataAnnotations;

namespace TaklaNew.API.Dtos
{
    public class BankUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
