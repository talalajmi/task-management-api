using Microsoft.Extensions.Logging;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Infrastructure.Services;

public class TaskCleanupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TaskCleanupService> _logger;

    public TaskCleanupService(IUnitOfWork unitOfWork, ILogger<TaskCleanupService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // Finds tasks that are overdue and still not done
    // Runs as a scheduled background job
    public async Task LogOverdueTasksAsync()
    {
        _logger.LogInformation("Running overdue task check at {Time}", DateTime.UtcNow);

        var allTasks = await _unitOfWork.Tasks.GetAllAsync();

        var overdueTasks = allTasks
            .Where(t =>
                t.DueDate.HasValue && t.DueDate < DateTime.UtcNow && t.Status != TaskItemStatus.Done
            )
            .ToList();

        if (!overdueTasks.Any())
        {
            _logger.LogInformation("No overdue tasks found.");
            return;
        }

        _logger.LogWarning(
            "{Count} overdue task(s) found: {Titles}",
            overdueTasks.Count,
            string.Join(", ", overdueTasks.Select(t => t.Title))
        );
    }
}
