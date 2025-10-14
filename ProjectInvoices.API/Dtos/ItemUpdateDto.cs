using System.ComponentModel.DataAnnotations;

namespace TaklaNew.API.Dtos
{
    public class ItemUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string unit { get; set; }

        public bool AffectInventory { get; set; }
    }
}
