using System.ComponentModel.DataAnnotations;

namespace ProjectInvoices.API.Dtos
{
    public class ItemCreationDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Unit { get; set; }
        
        public bool AffectInventory { get; set; }
    }
}
