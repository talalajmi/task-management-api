using Microsoft.AspNetCore.SignalR;
using TaskManagement.API.Hubs;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Infrastructure.Services;

public class NotificationService(IHubContext<TaskHub> hubContext) : INotificationService
{
    private readonly IHubContext<TaskHub> _hubContext = hubContext;

    public async Task NotifyTaskCreated(string projectId, TaskDto task)
    {
        await _hubContext
            .Clients.Group($"project:{projectId.ToUpper()}")
            .SendAsync("TaskCreated", task);
    }

    public async Task NotifyTaskUpdated(string projectId, TaskDto task)
    {
        await _hubContext
            .Clients.Group($"project:{projectId.ToUpper()}")
            .SendAsync("TaskUpdated", task);
    }

    public async Task NotifyTaskDeleted(string projectId, Guid taskId)
    {
        await _hubContext
            .Clients.Group($"project:{projectId.ToUpper()}")
            .SendAsync("TaskDeleted", taskId);
    }
}
