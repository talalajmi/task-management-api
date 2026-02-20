public interface ITaskRepository : IRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId);
    Task<IEnumerable<TaskItem>> GetByAssignedUserAsync(Guid userId);
    Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskItemStatus status);
}