using System.ComponentModel.DataAnnotations;

namespace ProjectInvoices.API.Dtos
{
    public class ProjectInvoiceItemUpdateDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string Unit { get; set; }
        public double Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
