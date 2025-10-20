namespace TaskTracker.Application.DTOs.Tasks;

using TaskTracker.Domain.Enums;

public sealed record TaskDto(
    int Id,
    string Title,
    string Description,
    int ProjectId,
    int TaskGroupId,
    DateTime DueDate,
    DateTime CreatedAt,
    TaskStatus Status,
    TaskPriority Priority,
    IReadOnlyCollection<TaskParticipantDto> Assignees,
    IReadOnlyCollection<TaskParticipantDto> Watchers,
    string RowVersion);
