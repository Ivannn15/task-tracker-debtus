namespace TaskTracker.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("tasks");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.Priority)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(t => t.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasMany(t => t.Assignees)
            .WithOne(a => a.Task!)
            .HasForeignKey(a => a.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Watchers)
            .WithOne(w => w.Task!)
            .HasForeignKey(w => w.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
