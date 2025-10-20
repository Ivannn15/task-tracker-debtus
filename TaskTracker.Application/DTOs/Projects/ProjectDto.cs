namespace TaskTracker.Application.DTOs.Projects;

public sealed record ProjectDto(
    int Id,
    string Name,
    string Description,
    int ProjectLeadId,
    int ProjectManagerId);
