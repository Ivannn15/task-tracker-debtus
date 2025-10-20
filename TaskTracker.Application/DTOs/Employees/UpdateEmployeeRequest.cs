namespace TaskTracker.Application.DTOs.Employees;

using TaskTracker.Domain.Enums;

public sealed record UpdateEmployeeRequest(
    string LastName,
    string FirstName,
    string MiddleName,
    string UserName,
    EmployeeRole Role);
