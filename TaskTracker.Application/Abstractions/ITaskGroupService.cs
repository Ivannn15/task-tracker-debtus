namespace TaskTracker.Application.Abstractions;

using TaskTracker.Application.DTOs.TaskGroups;

public interface ITaskGroupService
{
    Task<IReadOnlyCollection<TaskGroupDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<TaskGroupDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<TaskGroupDto> CreateAsync(CreateTaskGroupRequest request, CancellationToken cancellationToken);

    Task<TaskGroupDto> UpdateAsync(int id, UpdateTaskGroupRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
