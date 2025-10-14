using System.ComponentModel.DataAnnotations;

namespace TaklaNew.API.Dtos
{
    public class BankCreationDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
