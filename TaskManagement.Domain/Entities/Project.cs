public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Foreign key - who owns this project?
    public Guid OwnerId { get; set; }
    
    // Navigation property (EF Core uses this to JOIN tables)
    public AppUser Owner { get; set; } = null!;
    
    // One project has many tasks
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}