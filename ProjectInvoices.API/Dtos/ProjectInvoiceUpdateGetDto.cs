using System.ComponentModel.DataAnnotations;

namespace TaklaNew.API.Dtos
{
    public class ProjectInvoiceUpdateGetDto
    {
        public int Id { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime Date { get; set; }
        public int ProjectId { get; set; }
        public int SupplierId { get; set; }
        public short State { get; set; }
        public IList<ProjectInvoiceItemUpdateGetDto> Items { get; set; } = new List<ProjectInvoiceItemUpdateGetDto>();
    }
}
