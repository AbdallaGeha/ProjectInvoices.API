using ProjectInvoices.API.Domain.Enums;
using ProjectInvoices.API.Domain;

namespace ProjectInvoices.API.Dtos
{
    public class ProjectInvoiceViewRequestDto
    {
        public int? Id { get; set; }
        public string? Reference { get; set; }
        public int? ProjectId { get; set; }
        public int? SupplierId { get; set; }
        public int? State { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        
    }
}
