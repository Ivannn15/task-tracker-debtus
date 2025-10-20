namespace TaskTracker.Application.Abstractions;

using TaskTracker.Application.DTOs.Projects;

public interface IProjectService
{
    Task<IReadOnlyCollection<ProjectDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<ProjectDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<ProjectDto> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken);

    Task<ProjectDto> UpdateAsync(int id, UpdateProjectRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
