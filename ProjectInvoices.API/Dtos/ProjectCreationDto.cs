using System.ComponentModel.DataAnnotations;

namespace TaklaNew.API.Dtos
{
    public class ProjectCreationDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public short State { get; set; }
    }
}
