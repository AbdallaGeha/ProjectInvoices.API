using TaklaNew.API.Domain.Enums;
using TaklaNew.API.Domain;

namespace TaklaNew.API.Dtos
{
    public class ProjectInvoiceViewDto
    {
        public int Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string Date { get; set; }
        public string Project { get; set; }
        public string Supplier { get; set; }
        public string State { get; set; }
        public decimal Amount { get; set; }
        
    }
}
