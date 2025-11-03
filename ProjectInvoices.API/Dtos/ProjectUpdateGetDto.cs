using System.ComponentModel.DataAnnotations;

namespace ProjectInvoices.API.Dtos
{
    public class ProjectUpdateGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public short State { get; set; }
    }
}
