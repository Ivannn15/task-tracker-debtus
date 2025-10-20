namespace TaskTracker.Domain.Entities;

using TaskTracker.Domain.Enums;

public class Employee
{
    public int Id { get; set; }

    public string LastName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string MiddleName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public EmployeeRole Role { get; set; }

    public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public ICollection<TaskWatcher> TaskWatchers { get; set; } = new List<TaskWatcher>();

    public ICollection<Project> ManagedProjects { get; set; } = new List<Project>();

    public ICollection<Project> LedProjects { get; set; } = new List<Project>();
}
