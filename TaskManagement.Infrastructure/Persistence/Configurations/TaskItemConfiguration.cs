using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title).IsRequired().HasMaxLength(300);

        builder.Property(t => t.Description).HasMaxLength(2000);

        // Store enums as strings in the database, not integers.
        // Why? If you store as int, the database shows "2" — meaningless.
        // If you store as string, the database shows "InProgress" — readable.
        // This makes debugging and direct database queries much easier.
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(50);

        builder.Property(t => t.Priority).HasConversion<string>().HasMaxLength(50);

        // A task belongs to one project
        builder
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        // Cascade means: if a project is deleted, all its tasks are deleted too.
        // This makes logical sense — orphan tasks serve no purpose.

        // A task can be assigned to a user (optional — nullable)
        builder
            .HasOne(t => t.AssignedTo)
            .WithMany()
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);
        // SetNull means: if the assigned user is deleted,
        // the task remains but AssignedToId becomes null.
        // The task isn't lost, just unassigned.
    }
}
