namespace TaskTracker.Application.DTOs.Tasks;

using TaskTracker.Domain.Enums;

public sealed record UpdateTaskRequest(
    string Title,
    string Description,
    DateTime DueDate,
    TaskPriority Priority);
