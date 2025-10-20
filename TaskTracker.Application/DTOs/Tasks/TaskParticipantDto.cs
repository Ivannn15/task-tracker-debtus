namespace TaskTracker.Application.DTOs.Tasks;

public sealed record TaskParticipantDto(
    int EmployeeId,
    string LastName,
    string FirstName,
    string MiddleName,
    string UserName);
