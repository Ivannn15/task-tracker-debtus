namespace TaskTracker.Application.Abstractions;

using TaskTracker.Application.DTOs.Employees;

public interface IEmployeeService
{
    Task<IReadOnlyCollection<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken);

    Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
