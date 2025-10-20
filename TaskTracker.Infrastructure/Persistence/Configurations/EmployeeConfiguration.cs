namespace TaskTracker.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Domain.Entities;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.LastName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(e => e.FirstName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(e => e.MiddleName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(e => e.UserName)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(e => e.UserName)
            .IsUnique();

        builder.Property(e => e.Role)
            .HasConversion<int>()
            .IsRequired();

        builder.HasMany(e => e.TaskAssignments)
            .WithOne(a => a.Employee!)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.TaskWatchers)
            .WithOne(w => w.Employee!)
            .HasForeignKey(w => w.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ManagedProjects)
            .WithOne(p => p.ProjectManager!)
            .HasForeignKey(p => p.ProjectManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.LedProjects)
            .WithOne(p => p.ProjectLead!)
            .HasForeignKey(p => p.ProjectLeadId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
