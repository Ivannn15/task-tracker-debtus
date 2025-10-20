namespace TaskTracker.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Domain.Entities;

public sealed class TaskWatcherConfiguration : IEntityTypeConfiguration<TaskWatcher>
{
    public void Configure(EntityTypeBuilder<TaskWatcher> builder)
    {
        builder.ToTable("task_watchers");
        builder.HasKey(w => new { w.TaskItemId, w.EmployeeId });

        builder.HasOne(w => w.Task)
            .WithMany(t => t.Watchers)
            .HasForeignKey(w => w.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.Employee)
            .WithMany(e => e.TaskWatchers)
            .HasForeignKey(w => w.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
