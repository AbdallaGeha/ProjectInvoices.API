namespace TaklaNew.API.Dtos
{
    public class SuppliersPaginateDto
    {
        public IEnumerable<SupplierDto> Suppliers { get; set; } = new List<SupplierDto>();

        public int TotalRecords { get; set; }
    }
}
