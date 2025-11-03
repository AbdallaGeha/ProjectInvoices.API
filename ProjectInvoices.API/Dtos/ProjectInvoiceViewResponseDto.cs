using ProjectInvoices.API.Domain.Enums;
using ProjectInvoices.API.Domain;

namespace ProjectInvoices.API.Dtos
{
    public class ProjectInvoiceViewResponseDto
    {
        public List<ProjectInvoiceViewDto> Data { get; set; }
        public int TotalRecords { get; set; }
    }
}
