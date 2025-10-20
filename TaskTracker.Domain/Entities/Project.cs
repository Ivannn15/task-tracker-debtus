namespace TaskTracker.Domain.Entities;

public class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int ProjectLeadId { get; set; }

    public Employee? ProjectLead { get; set; }

    public int ProjectManagerId { get; set; }

    public Employee? ProjectManager { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
