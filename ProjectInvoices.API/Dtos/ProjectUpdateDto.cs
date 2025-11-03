using System.ComponentModel.DataAnnotations;

namespace ProjectInvoices.API.Dtos
{
    public class ProjectUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public short State { get; set; }
    }
}
