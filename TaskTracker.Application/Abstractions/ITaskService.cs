namespace TaskTracker.Application.Abstractions;

using TaskTracker.Application.DTOs.Tasks;

public interface ITaskService
{
    Task<IReadOnlyCollection<TaskDto>> GetAllAsync(int? employeeId, int? taskGroupId, int? projectId, CancellationToken cancellationToken);

    Task<TaskDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken);

    Task<TaskDto> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken);

    Task<TaskDto> AddAssigneeAsync(int id, ModifyTaskParticipantRequest request, CancellationToken cancellationToken);

    Task<TaskDto> AddWatcherAsync(int id, ModifyTaskParticipantRequest request, CancellationToken cancellationToken);

    Task<TaskDto> UpdateStatusAsync(int id, UpdateTaskStatusRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
