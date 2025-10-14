using TaklaNew.API.Domain.Enums;
using TaklaNew.API.Domain;

namespace TaklaNew.API.Dtos
{
    public class ProjectInvoiceViewResponseDto
    {
        public List<ProjectInvoiceViewDto> Data { get; set; }
        public int TotalRecords { get; set; }
    }
}
