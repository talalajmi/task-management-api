public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }

    // Which project does this task belong to?
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    // Who is assigned to this task?
    public Guid? AssignedToId { get; set; }
    public AppUser? AssignedTo { get; set; }
}