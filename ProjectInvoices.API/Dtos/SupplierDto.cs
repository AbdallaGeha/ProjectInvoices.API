using System.ComponentModel.DataAnnotations;

namespace TaklaNew.API.Dtos
{
    public class SupplierDto
    {
        public int Id { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastModifiedDate { get; set; }
        
        public string Name { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }
    }
}
