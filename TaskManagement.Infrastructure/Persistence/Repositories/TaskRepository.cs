using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Infrastructure.Persistence.Repositories;

public class TaskRepository(ApplicationDbContext context)
    : Repository<TaskItem>(context),
        ITaskRepository
{
    public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId)
    {
        return await _dbSet
            .AsNoTracking()
            // Include loads the related entity in the same query (SQL JOIN)
            // Without this, AssignedTo would be null even if it exists
            .Include(t => t.AssignedTo)
            .Where(t => t.ProjectId == projectId)
            // Always return in a predictable order
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetByAssignedUserAsync(Guid userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(t => t.Project)
            .Where(t => t.AssignedToId == userId)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetByStatusAsync(Domain.Enums.TaskItemStatus status)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(t => t.AssignedTo)
            .Include(t => t.Project)
            .Where(t => t.Status == status)
            .ToListAsync();
    }
}
