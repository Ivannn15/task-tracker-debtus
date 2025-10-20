namespace TaskTracker.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Abstractions;
using TaskTracker.Application.DTOs.Employees;
using TaskTracker.Application.Exceptions;
using TaskTracker.Infrastructure.Extensions;
using TaskTracker.Infrastructure.Persistence;

public sealed class EmployeeService(TaskTrackerDbContext dbContext) : IEmployeeService
{
    private readonly TaskTrackerDbContext _dbContext = dbContext;

    public async Task<IReadOnlyCollection<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var employees = await _dbContext.Employees
            .AsNoTracking()
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);

        return employees.Select(e => e.ToDto()).ToArray();
    }

    public async Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await _dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException("Сотрудник не найден", $"Сотрудник с идентификатором {id} не найден.");
        }

        return employee.ToDto();
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var hasDuplicateUserName = await _dbContext.Employees
            .AnyAsync(e => e.UserName == request.UserName, cancellationToken);

        if (hasDuplicateUserName)
        {
            throw new ConflictException("Имя пользователя занято", $"Сотрудник с именем пользователя '{request.UserName}' уже существует.");
        }

        var employee = new Domain.Entities.Employee
        {
            LastName = request.LastName.Trim(),
            FirstName = request.FirstName.Trim(),
            MiddleName = request.MiddleName.Trim(),
            UserName = request.UserName.Trim(),
            Role = request.Role
        };

        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return employee.ToDto();
    }

    public async Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException("Сотрудник не найден", $"Сотрудник с идентификатором {id} не найден.");
        }

        var normalizedUserName = request.UserName.Trim();
        var hasDuplicateUserName = await _dbContext.Employees
            .AnyAsync(e => e.Id != id && e.UserName == normalizedUserName, cancellationToken);

        if (hasDuplicateUserName)
        {
            throw new ConflictException("Имя пользователя занято", $"Сотрудник с именем пользователя '{request.UserName}' уже существует.");
        }

        employee.LastName = request.LastName.Trim();
        employee.FirstName = request.FirstName.Trim();
        employee.MiddleName = request.MiddleName.Trim();
        employee.UserName = normalizedUserName;
        employee.Role = request.Role;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return employee.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await _dbContext.Employees
            .Include(e => e.ManagedProjects)
            .Include(e => e.LedProjects)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException("Сотрудник не найден", $"Сотрудник с идентификатором {id} не найден.");
        }

        if (employee.ManagedProjects.Any() || employee.LedProjects.Any())
        {
            throw new ConflictException(
                "Удаление невозможно",
                "Нельзя удалить сотрудника, который назначен руководителем или менеджером проекта.");
        }

        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
