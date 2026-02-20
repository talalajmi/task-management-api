using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Interfaces;

public interface INotificationService
{
    // Notify all users viewing this project that a task was created
    Task NotifyTaskCreated(string projectId, TaskDto task);

    // Notify all users viewing this project that a task was updated
    Task NotifyTaskUpdated(string projectId, TaskDto task);

    // Notify all users viewing this project that a task was deleted
    Task NotifyTaskDeleted(string projectId, Guid taskId);
}
