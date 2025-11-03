using System.ComponentModel.DataAnnotations;

namespace ProjectInvoices.API.Dtos
{
    public class ProjectCreationDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public short State { get; set; }
    }
}
