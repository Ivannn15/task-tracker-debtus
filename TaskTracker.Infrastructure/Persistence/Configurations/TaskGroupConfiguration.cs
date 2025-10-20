namespace TaskTracker.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Domain.Entities;

public sealed class TaskGroupConfiguration : IEntityTypeConfiguration<TaskGroup>
{
    public void Configure(EntityTypeBuilder<TaskGroup> builder)
    {
        builder.ToTable("task_groups");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasMany(g => g.Tasks)
            .WithOne(t => t.TaskGroup!)
            .HasForeignKey(t => t.TaskGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
