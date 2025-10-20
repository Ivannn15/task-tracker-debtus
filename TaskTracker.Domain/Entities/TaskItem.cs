namespace TaskTracker.Domain.Entities;

using TaskTracker.Domain.Enums;

public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int ProjectId { get; set; }

    public Project? Project { get; set; }

    public int TaskGroupId { get; set; }

    public TaskGroup? TaskGroup { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public TaskStatus Status { get; set; }

    public TaskPriority Priority { get; set; }

    public ICollection<TaskAssignment> Assignees { get; set; } = new List<TaskAssignment>();

    public ICollection<TaskWatcher> Watchers { get; set; } = new List<TaskWatcher>();

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
