namespace TaklaNew.API.Dtos
{
    public class ProjectsPaginateDto
    {
        public IEnumerable<ProjectDto> Projects { get; set; } = new List<ProjectDto>();

        public int TotalRecords { get; set; }
    }
}
