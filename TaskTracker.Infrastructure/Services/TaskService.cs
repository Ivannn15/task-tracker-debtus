namespace TaskTracker.Infrastructure.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Abstractions;
using TaskTracker.Application.DTOs.Tasks;
using TaskTracker.Application.Exceptions;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;
using TaskTracker.Infrastructure.Extensions;
using TaskTracker.Infrastructure.Persistence;

public sealed class TaskService(TaskTrackerDbContext dbContext) : ITaskService
{
    private readonly TaskTrackerDbContext _dbContext = dbContext;

    public async Task<IReadOnlyCollection<TaskDto>> GetAllAsync(int? employeeId, int? taskGroupId, int? projectId, CancellationToken cancellationToken)
    {
        var query = BuildTaskQuery(asTracking: false);

        if (employeeId.HasValue)
        {
            var id = employeeId.Value;
            query = query.Where(t => t.Assignees.Any(a => a.EmployeeId == id) || t.Watchers.Any(w => w.EmployeeId == id));
        }

        if (taskGroupId.HasValue)
        {
            query = query.Where(t => t.TaskGroupId == taskGroupId.Value);
        }

        if (projectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == projectId.Value);
        }

        var tasks = await query
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.Id)
            .ToListAsync(cancellationToken);

        return tasks.Select(t => t.ToDto()).ToArray();
    }

    public async Task<TaskDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var task = await LoadTaskAsync(id, asTracking: false, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException("Задача не найдена", $"Задача с идентификатором {id} не найдена.");
        }

        return task.ToDto();
    }

    public async Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        ValidateInitialStatus(request.Status);

        var assigneeIds = (request.AssigneeIds ?? Array.Empty<int>()).Distinct().ToArray();
        var watcherIds = (request.WatcherIds ?? Array.Empty<int>()).Distinct().ToArray();

        EnsureParticipantsDoNotOverlap(assigneeIds, watcherIds);

        await EnsureProjectExists(request.ProjectId, cancellationToken);
        await EnsureTaskGroupExists(request.TaskGroupId, cancellationToken);
        await EnsureEmployeesExist(assigneeIds.Concat(watcherIds), cancellationToken);

        var task = new TaskItem
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            ProjectId = request.ProjectId,
            TaskGroupId = request.TaskGroupId,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow,
            Status = request.Status,
            Priority = request.Priority,
            Assignees = assigneeIds.Select(id => new TaskAssignment { EmployeeId = id }).ToList(),
            Watchers = watcherIds.Select(id => new TaskWatcher { EmployeeId = id }).ToList()
        };

        _dbContext.Tasks.Add(task);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (await LoadTaskAsync(task.Id, asTracking: false, cancellationToken))!.ToDto();
    }

    public async Task<TaskDto> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await LoadTaskAsync(id, asTracking: true, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException("Задача не найдена", $"Задача с идентификатором {id} не найдена.");
        }

        task.Title = request.Title.Trim();
        task.Description = request.Description.Trim();
        task.DueDate = request.DueDate;
        task.Priority = request.Priority;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return (await LoadTaskAsync(id, asTracking: false, cancellationToken))!.ToDto();
    }

    public async Task<TaskDto> AddAssigneeAsync(int id, ModifyTaskParticipantRequest request, CancellationToken cancellationToken)
    {
        var task = await LoadTaskAsync(id, asTracking: true, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException("Задача не найдена", $"Задача с идентификатором {id} не найдена.");
        }

        await EnsureEmployeesExist([request.EmployeeId], cancellationToken);

        if (task.Assignees.Any(a => a.EmployeeId == request.EmployeeId))
        {
            throw new ConflictException("Исполнитель уже добавлен", "Указанный сотрудник уже является исполнителем задачи.");
        }

        if (task.Watchers.Any(w => w.EmployeeId == request.EmployeeId))
        {
            throw new ConflictException("Конфликт ролей", "Нельзя добавить сотрудника в исполнители, так как он является наблюдателем задачи.");
        }

        task.Assignees.Add(new TaskAssignment { EmployeeId = request.EmployeeId });
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (await LoadTaskAsync(id, asTracking: false, cancellationToken))!.ToDto();
    }

    public async Task<TaskDto> AddWatcherAsync(int id, ModifyTaskParticipantRequest request, CancellationToken cancellationToken)
    {
        var task = await LoadTaskAsync(id, asTracking: true, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException("Задача не найдена", $"Задача с идентификатором {id} не найдена.");
        }

        await EnsureEmployeesExist([request.EmployeeId], cancellationToken);

        if (task.Watchers.Any(w => w.EmployeeId == request.EmployeeId))
        {
            throw new ConflictException("Наблюдатель уже добавлен", "Указанный сотрудник уже является наблюдателем задачи.");
        }

        if (task.Assignees.Any(a => a.EmployeeId == request.EmployeeId))
        {
            throw new ConflictException("Конфликт ролей", "Нельзя добавить сотрудника в наблюдатели, так как он является исполнителем задачи.");
        }

        task.Watchers.Add(new TaskWatcher { EmployeeId = request.EmployeeId });
        await _dbContext.SaveChangesAsync(cancellationToken);

        return (await LoadTaskAsync(id, asTracking: false, cancellationToken))!.ToDto();
    }

    public async Task<TaskDto> UpdateStatusAsync(int id, UpdateTaskStatusRequest request, CancellationToken cancellationToken)
    {
        var task = await LoadTaskAsync(id, asTracking: true, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException("Задача не найдена", $"Задача с идентификатором {id} не найдена.");
        }

        var targetStatus = request.Status;
        if (!task.Status.CanTransitionTo(targetStatus))
        {
            throw new ValidationException(
                "Неверный переход статуса",
                $"Нельзя перейти из статуса {task.Status} в статус {targetStatus}.");
        }

        var rowVersion = DecodeRowVersion(request.RowVersion);
        _dbContext.Entry(task).Property(t => t.RowVersion).OriginalValue = rowVersion;

        task.Status = targetStatus;

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("Конфликт обновления", "Задача была изменена другим пользователем. Повторите попытку.");
        }

        return (await LoadTaskAsync(id, asTracking: false, cancellationToken))!.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var task = await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Задача не найдена", $"Задача с идентификатором {id} не найдена.");
        }

        if (!task.Status.IsInitial() && !task.Status.IsFinal())
        {
            throw new ConflictException(
                "Удаление невозможно",
                "Удалять можно только задачи в начальных или конечных статусах.");
        }

        _dbContext.Tasks.Remove(task);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<TaskItem> BuildTaskQuery(bool asTracking)
    {
        var query = _dbContext.Tasks
            .Include(t => t.Assignees).ThenInclude(a => a.Employee)
            .Include(t => t.Watchers).ThenInclude(w => w.Employee)
            .AsQueryable();

        return asTracking ? query : query.AsNoTracking();
    }

    private Task<TaskItem?> LoadTaskAsync(int id, bool asTracking, CancellationToken cancellationToken)
    {
        return BuildTaskQuery(asTracking)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    private static void ValidateInitialStatus(TaskStatus status)
    {
        if (status is not (TaskStatus.Backlog or TaskStatus.Current))
        {
            throw new ValidationException(
                "Некорректный статус",
                "Новая задача может быть создана только в статусах 'Бэклог' или 'Текущая'.");
        }
    }

    private static void EnsureParticipantsDoNotOverlap(IReadOnlyCollection<int> assigneeIds, IReadOnlyCollection<int> watcherIds)
    {
        if (assigneeIds.Intersect(watcherIds).Any())
        {
            throw new ValidationException(
                "Конфликт ролей",
                "Сотрудник не может быть одновременно исполнителем и наблюдателем задачи.");
        }
    }

    private async Task EnsureProjectExists(int projectId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Projects.AnyAsync(p => p.Id == projectId, cancellationToken);
        if (!exists)
        {
            throw new ValidationException("Проект не найден", $"Проект с идентификатором {projectId} не найден.");
        }
    }

    private async Task EnsureTaskGroupExists(int groupId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.TaskGroups.AnyAsync(g => g.Id == groupId, cancellationToken);
        if (!exists)
        {
            throw new ValidationException("Группа задач не найдена", $"Группа задач с идентификатором {groupId} не найдена.");
        }
    }

    private async Task EnsureEmployeesExist(IEnumerable<int> employeeIds, CancellationToken cancellationToken)
    {
        var ids = employeeIds.Distinct().ToArray();
        if (ids.Length == 0)
        {
            return;
        }

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

    private static byte[] DecodeRowVersion(string rowVersion)
    {
        try
        {
            return Convert.FromBase64String(rowVersion);
        }
        catch (FormatException)
        {
            throw new ValidationException("Некорректная версия", "Невозможно обработать переданную версию данных.");
        }
    }
}
