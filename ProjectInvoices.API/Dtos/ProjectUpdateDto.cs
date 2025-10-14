using System.ComponentModel.DataAnnotations;

namespace TaklaNew.API.Dtos
{
    public class ProjectUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public short State { get; set; }
    }
}
