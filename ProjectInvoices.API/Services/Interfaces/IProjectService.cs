using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Dtos;

namespace ProjectInvoices.API.Services.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectUpdateGetDto> GetProjectByIdAsync(int id);
        Task<ProjectsPaginateDto> GetProjectsAsync(int page, int pageSize, string? search);
        Task AddProjectAsync(ProjectCreationDto Project);
        Task UpdateProjectAsync(int id, ProjectUpdateDto Project);
        Task DeleteProjectAsync(int id);
    }
}
