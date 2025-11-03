using ProjectInvoices.API.Domain;

namespace ProjectInvoices.API.Domain.IRepository
{
    /// <summary>
    /// Defines data access operations for projects
    /// </summary>
    public interface IProjectRepository
    {
        /// <summary>
        /// Add project to datastore
        /// </summary>
        Task AddProjectAsync(Project project);

        /// <summary>
        /// Delete project from datastore
        /// </summary>
        Task DeleteProjectAsync(Project project);

        /// <summary>
        /// Retrieves all projects from datastore
        /// </summary>
        Task<IList<Project>> GetAllProjectsAsync();

        /// <summary>
        /// Retrieves project from datastore by its id
        /// </summary>
        Task<Project?> GetProjectByIdAsync(int id);

        /// <summary>
        /// Retrieves projects from datastore by paging info and search keyword
        /// </summary>
        Task<IList<Project>> GetProjectsAsync(int page, int pageSize, string? search);

        /// <summary>
        /// Retrieves projects count from datastore by search keyword
        /// </summary>
        Task<int> GetTotalRecords(string? search);

        /// <summary>
        /// Check if project name exists in datastore
        /// </summary>
        Task<bool> IsExistingProjectAsync(string name);

        /// <summary>
        /// Update project in datastore
        /// </summary>
        Task UpdateProjectAsync();
    }
}