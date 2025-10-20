namespace TaskTracker.Application.DTOs.Tasks;

using TaskTracker.Domain.Enums;

public sealed record CreateTaskRequest(
    string Title,
    string Description,
    int ProjectId,
    int TaskGroupId,
    DateTime DueDate,
    TaskStatus Status,
    TaskPriority Priority,
    IReadOnlyCollection<int> AssigneeIds,
    IReadOnlyCollection<int> WatcherIds);
