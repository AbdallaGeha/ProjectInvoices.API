using System.ComponentModel.DataAnnotations;

namespace TaklaNew.API.Dtos
{
    public class ItemUpdateGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public bool AffectInventory { get; set; }
    }
}
