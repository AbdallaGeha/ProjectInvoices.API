namespace ProjectInvoices.API.Dtos
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
    }
}
