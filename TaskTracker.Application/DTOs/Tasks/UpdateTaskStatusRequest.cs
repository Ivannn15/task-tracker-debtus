namespace TaskTracker.Application.DTOs.Tasks;

using TaskTracker.Domain.Enums;

public sealed record UpdateTaskStatusRequest(
    TaskStatus Status,
    string RowVersion);
