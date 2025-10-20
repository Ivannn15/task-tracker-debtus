namespace TaskTracker.Domain.Entities;

public class TaskAssignment
{
    public int TaskItemId { get; set; }

    public TaskItem? Task { get; set; }

    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }
}
