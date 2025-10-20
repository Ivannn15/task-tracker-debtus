namespace TaskTracker.Infrastructure.Extensions;

using System;
using System.Linq;
using TaskTracker.Application.DTOs.Employees;
using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Application.DTOs.TaskGroups;
using TaskTracker.Application.DTOs.Tasks;
using TaskTracker.Domain.Entities;

internal static class EntityMappingExtensions
{
    public static EmployeeDto ToDto(this Employee employee) =>
        new(
            employee.Id,
            employee.LastName,
            employee.FirstName,
            employee.MiddleName,
            employee.UserName,
            employee.Role);

    public static ProjectDto ToDto(this Project project) =>
        new(
            project.Id,
            project.Name,
            project.Description,
            project.ProjectLeadId,
            project.ProjectManagerId);

    public static TaskGroupDto ToDto(this TaskGroup taskGroup) =>
        new(
            taskGroup.Id,
            taskGroup.Name);

    public static TaskParticipantDto ToParticipantDto(this Employee employee) =>
        new(
            employee.Id,
            employee.LastName,
            employee.FirstName,
            employee.MiddleName,
            employee.UserName);

    public static TaskDto ToDto(this TaskItem task) =>
        new(
            task.Id,
            task.Title,
            task.Description,
            task.ProjectId,
            task.TaskGroupId,
            task.DueDate,
            task.CreatedAt,
            task.Status,
            task.Priority,
            task.Assignees.Select(a => a.Employee!.ToParticipantDto()).ToArray(),
            task.Watchers.Select(w => w.Employee!.ToParticipantDto()).ToArray(),
            Convert.ToBase64String(task.RowVersion));
}
