namespace TaskTracker.Infrastructure.Services;

using System.Linq;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Abstractions;
using TaskTracker.Application.DTOs.TaskGroups;
using TaskTracker.Application.Exceptions;
using TaskTracker.Domain.Enums;
using TaskTracker.Infrastructure.Extensions;
using TaskTracker.Infrastructure.Persistence;

public sealed class TaskGroupService(TaskTrackerDbContext dbContext) : ITaskGroupService
{
    private readonly TaskTrackerDbContext _dbContext = dbContext;

    public async Task<IReadOnlyCollection<TaskGroupDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var groups = await _dbContext.TaskGroups
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);

        return groups.Select(g => g.ToDto()).ToArray();
    }

    public async Task<TaskGroupDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var group = await _dbContext.TaskGroups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Группа задач не найдена", $"Группа задач с идентификатором {id} не найдена.");
        }

        return group.ToDto();
    }

    public async Task<TaskGroupDto> CreateAsync(CreateTaskGroupRequest request, CancellationToken cancellationToken)
    {
        var group = new Domain.Entities.TaskGroup
        {
            Name = request.Name.Trim()
        };

        _dbContext.TaskGroups.Add(group);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return group.ToDto();
    }

    public async Task<TaskGroupDto> UpdateAsync(int id, UpdateTaskGroupRequest request, CancellationToken cancellationToken)
    {
        var group = await _dbContext.TaskGroups
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Группа задач не найдена", $"Группа задач с идентификатором {id} не найдена.");
        }

        group.Name = request.Name.Trim();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return group.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var group = await _dbContext.TaskGroups
            .Include(g => g.Tasks)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

        if (group is null)
        {
            throw new NotFoundException("Группа задач не найдена", $"Группа задач с идентификатором {id} не найдена.");
        }

        if (group.Tasks.Any(t => t.Status is not TaskStatus.Cancelled)
            && group.Tasks.Any())
        {
            throw new ConflictException(
                "Удаление невозможно",
                "Группу можно удалить только если в ней нет задач или все задачи отменены.");
        }

        if (group.Tasks.Any())
        {
            _dbContext.Tasks.RemoveRange(group.Tasks);
        }

        _dbContext.TaskGroups.Remove(group);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
