using System.ComponentModel.DataAnnotations;

namespace TaklaNew.API.Dtos
{
    public class ProjectInvoiceItemCreationDto
    {
        public int ItemId { get; set; }
        public string Unit { get; set; }
        public double Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
