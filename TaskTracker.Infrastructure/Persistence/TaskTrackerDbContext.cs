namespace TaskTracker.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;
using TaskTracker.Infrastructure.Persistence.Configurations;
using TaskTracker.Infrastructure.Persistence.Seed;

public sealed class TaskTrackerDbContext(DbContextOptions<TaskTrackerDbContext> options)
    : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<TaskGroup> TaskGroups => Set<TaskGroup>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new TaskGroupConfiguration());
        modelBuilder.ApplyConfiguration(new TaskItemConfiguration());
        modelBuilder.ApplyConfiguration(new TaskAssignmentConfiguration());
        modelBuilder.ApplyConfiguration(new TaskWatcherConfiguration());

        SeedData.Apply(modelBuilder);
    }
}
