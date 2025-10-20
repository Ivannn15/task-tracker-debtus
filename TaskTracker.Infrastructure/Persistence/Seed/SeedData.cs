namespace TaskTracker.Infrastructure.Persistence.Seed;

using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

public static class SeedData
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        var employees = GenerateEmployees();
        modelBuilder.Entity<Employee>().HasData(employees);

        var projects = GenerateProjects();
        modelBuilder.Entity<Project>().HasData(projects);

        var taskGroups = GenerateTaskGroups();
        modelBuilder.Entity<TaskGroup>().HasData(taskGroups);

        var tasks = GenerateTasks();
        modelBuilder.Entity<TaskItem>().HasData(tasks);

        var assignments = GenerateAssignments();
        modelBuilder.Entity<TaskAssignment>().HasData(assignments);

        var watchers = GenerateWatchers();
        modelBuilder.Entity<TaskWatcher>().HasData(watchers);
    }

    private static IReadOnlyCollection<Employee> GenerateEmployees()
    {
        var employees = new List<Employee>(capacity: 20);
        var roles = new[]
        {
            EmployeeRole.Manager,
            EmployeeRole.Analyst,
            EmployeeRole.Developer,
            EmployeeRole.Tester
        };

        for (var i = 1; i <= 20; i++)
        {
            var role = roles[(i - 1) % roles.Length];
            employees.Add(new Employee
            {
                Id = i,
                LastName = $"Фамилия {i}",
                FirstName = $"Имя {i}",
                MiddleName = $"Отчество {i}",
                UserName = $"user{i:D2}",
                Role = role
            });
        }

        return employees;
    }

    private static IReadOnlyCollection<Project> GenerateProjects()
    {
        var projects = new List<Project>(capacity: 20);

        for (var i = 1; i <= 20; i++)
        {
            projects.Add(new Project
            {
                Id = i,
                Name = $"Проект {i}",
                Description = $"Описание проекта {i}",
                ProjectLeadId = ((i - 1) % 20) + 1,
                ProjectManagerId = ((i + 4 - 1) % 20) + 1
            });
        }

        return projects;
    }

    private static IReadOnlyCollection<TaskGroup> GenerateTaskGroups()
    {
        var groups = new List<TaskGroup>(capacity: 20);

        for (var i = 1; i <= 20; i++)
        {
            groups.Add(new TaskGroup
            {
                Id = i,
                Name = $"Группа {i}"
            });
        }

        return groups;
    }

    private static IReadOnlyCollection<TaskItem> GenerateTasks()
    {
        var tasks = new List<TaskItem>(capacity: 20);
        var baseCreatedAt = new DateTime(2024, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        var statuses = new[]
        {
            TaskStatus.Backlog,
            TaskStatus.Current,
            TaskStatus.InProgress,
            TaskStatus.InReview,
            TaskStatus.Testing,
            TaskStatus.Done
        };

        var priorities = new[]
        {
            TaskPriority.Low,
            TaskPriority.Medium,
            TaskPriority.High,
            TaskPriority.Critical,
            TaskPriority.Blocker
        };

        for (var i = 1; i <= 20; i++)
        {
            tasks.Add(new TaskItem
            {
                Id = i,
                Title = $"Задача {i}",
                Description = $"Описание задачи {i}",
                ProjectId = ((i - 1) % 20) + 1,
                TaskGroupId = i,
                DueDate = baseCreatedAt.AddDays(i + 5),
                CreatedAt = baseCreatedAt.AddDays(-(21 - i)),
                Status = statuses[(i - 1) % statuses.Length],
                Priority = priorities[(i - 1) % priorities.Length]
            });
        }

        return tasks;
    }

    private static IReadOnlyCollection<TaskAssignment> GenerateAssignments()
    {
        var assignments = new List<TaskAssignment>(capacity: 40);

        for (var i = 1; i <= 20; i++)
        {
            var primaryEmployeeId = GetPrimaryAssigneeId(i);
            var secondaryEmployeeId = GetSecondaryAssigneeId(i);

            assignments.Add(new TaskAssignment
            {
                TaskItemId = i,
                EmployeeId = primaryEmployeeId
            });

            assignments.Add(new TaskAssignment
            {
                TaskItemId = i,
                EmployeeId = secondaryEmployeeId
            });
        }

        return assignments;
    }

    private static IReadOnlyCollection<TaskWatcher> GenerateWatchers()
    {
        var watchers = new List<TaskWatcher>(capacity: 40);

        for (var i = 1; i <= 20; i++)
        {
            var firstWatcherId = ((i + 3) % 20) + 1;
            var secondWatcherId = ((i + 7) % 20) + 1;

            if (IsAssignee(i, firstWatcherId))
            {
                firstWatcherId = ((firstWatcherId + 5) % 20) + 1;
            }

            if (IsAssignee(i, secondWatcherId) || secondWatcherId == firstWatcherId)
            {
                secondWatcherId = ((secondWatcherId + 6) % 20) + 1;
            }

            watchers.Add(new TaskWatcher
            {
                TaskItemId = i,
                EmployeeId = firstWatcherId
            });

            watchers.Add(new TaskWatcher
            {
                TaskItemId = i,
                EmployeeId = secondWatcherId
            });
        }

        return watchers;
    }

    private static bool IsAssignee(int taskId, int employeeId)
    {
        var primaryEmployeeId = GetPrimaryAssigneeId(taskId);
        var secondaryEmployeeId = GetSecondaryAssigneeId(taskId);

        return employeeId == primaryEmployeeId || employeeId == secondaryEmployeeId;
    }

    private static int GetPrimaryAssigneeId(int taskId) => ((taskId - 1) % 10) + 1;

    private static int GetSecondaryAssigneeId(int taskId) => GetPrimaryAssigneeId(taskId) + 10;
}
