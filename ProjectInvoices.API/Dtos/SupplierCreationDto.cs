using System.ComponentModel.DataAnnotations;

namespace ProjectInvoices.API.Dtos
{
    public class SupplierCreationDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(50)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? Address { get; set; }
    }
}
