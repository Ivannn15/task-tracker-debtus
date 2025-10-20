namespace TaskTracker.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Domain.Entities;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(1024)
            .IsRequired();

        builder.HasOne(p => p.ProjectLead)
            .WithMany(e => e.LedProjects)
            .HasForeignKey(p => p.ProjectLeadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ProjectManager)
            .WithMany(e => e.ManagedProjects)
            .HasForeignKey(p => p.ProjectManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Tasks)
            .WithOne(t => t.Project!)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
