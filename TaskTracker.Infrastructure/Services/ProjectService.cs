namespace TaskTracker.Infrastructure.Services;

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Abstractions;
using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Application.Exceptions;
using TaskTracker.Domain.Enums;
using TaskTracker.Infrastructure.Extensions;
using TaskTracker.Infrastructure.Persistence;

public sealed class ProjectService(TaskTrackerDbContext dbContext) : IProjectService
{
    private readonly TaskTrackerDbContext _dbContext = dbContext;

    public async Task<IReadOnlyCollection<ProjectDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var projects = await _dbContext.Projects
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return projects.Select(p => p.ToDto()).ToArray();
    }

    public async Task<ProjectDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (project is null)
        {
            throw new NotFoundException("Проект не найден", $"Проект с идентификатором {id} не найден.");
        }

        return project.ToDto();
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectRequest request, CancellationToken cancellationToken)
    {
        await EnsureEmployeesExist([request.ProjectLeadId, request.ProjectManagerId], cancellationToken);

        var project = new Domain.Entities.Project
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            ProjectLeadId = request.ProjectLeadId,
            ProjectManagerId = request.ProjectManagerId
        };

        _dbContext.Projects.Add(project);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return project.ToDto();
    }

    public async Task<ProjectDto> UpdateAsync(int id, UpdateProjectRequest request, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (project is null)
        {
            throw new NotFoundException("Проект не найден", $"Проект с идентификатором {id} не найден.");
        }

        await EnsureEmployeesExist([request.ProjectLeadId, request.ProjectManagerId], cancellationToken);

        project.Name = request.Name.Trim();
        project.Description = request.Description.Trim();
        project.ProjectLeadId = request.ProjectLeadId;
        project.ProjectManagerId = request.ProjectManagerId;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return project.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (project is null)
        {
            throw new NotFoundException("Проект не найден", $"Проект с идентификатором {id} не найден.");
        }

        if (project.Tasks.Any(t => t.Status is not TaskStatus.Cancelled)
            && project.Tasks.Any())
        {
            throw new ConflictException(
                "Удаление невозможно",
                "Проект можно удалить только если в нём нет задач или все задачи отменены.");
        }

        if (project.Tasks.Any())
        {
            _dbContext.Tasks.RemoveRange(project.Tasks);
        }

        _dbContext.Projects.Remove(project);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureEmployeesExist(IEnumerable<int> employeeIds, CancellationToken cancellationToken)
    {
        var ids = employeeIds.ToArray();
        var existingIds = await _dbContext.Employees
            .Where(e => ids.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);

        var missingIds = ids.Except(existingIds).ToArray();
        if (missingIds.Length > 0)
        {
            throw new ValidationException(
                "Некорректные сотрудники",
                $"Следующие сотрудники не найдены: {string.Join(", ", missingIds)}");
        }
    }
}
