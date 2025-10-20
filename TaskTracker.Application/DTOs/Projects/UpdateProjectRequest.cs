namespace TaskTracker.Application.DTOs.Projects;

public sealed record UpdateProjectRequest(
    string Name,
    string Description,
    int ProjectLeadId,
    int ProjectManagerId);
