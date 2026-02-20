using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    // DbContext needs options passed in — this is how EF Core knows
    // which database to connect to. We pass this in from Program.cs
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Each DbSet = one table in SQL Server
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // This line automatically scans the Infrastructure assembly
        // and applies ALL IEntityTypeConfiguration classes it finds.
        // This means you never have to manually register each config —
        // just create a new config class and it gets picked up automatically.
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly
        );

        base.OnModelCreating(modelBuilder);
    }

    // This override adds automatic audit tracking.
    // Every time you call SaveChangesAsync(), this runs first
    // and sets UpdatedAt on any modified entity automatically.
    // You never have to manually set UpdatedAt anywhere in your code.
    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}