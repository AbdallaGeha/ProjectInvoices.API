using System.ComponentModel.DataAnnotations;

namespace ProjectInvoices.API.Dtos
{
    public class SupplierUpdateGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}
