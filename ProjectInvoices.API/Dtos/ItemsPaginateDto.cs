namespace TaklaNew.API.Dtos
{
    public class ItemsPaginateDto
    {
        public IEnumerable<ItemDto> Items { get; set; } = new List<ItemDto>();

        public int TotalRecords { get; set; }
    }
}
