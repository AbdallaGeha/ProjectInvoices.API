using System.ComponentModel.DataAnnotations;

namespace TaklaNew.API.Dtos
{
    public class ItemDto
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string AffectInventory { get; set; }
    }
}
