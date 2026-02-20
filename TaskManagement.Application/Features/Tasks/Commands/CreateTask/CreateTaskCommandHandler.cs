using MediatR;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;
    private readonly INotificationService _notifications;

    public CreateTaskCommandHandler(
        IUnitOfWork unitOfWork,
        ICacheService cache,
        INotificationService notifications
    )
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
        _notifications = notifications;
    }

    public async Task<TaskDto> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken
    )
    {
        var projectExists = await _unitOfWork.Projects.ExistsAsync(request.ProjectId);

        if (!projectExists)
            throw new KeyNotFoundException($"Project {request.ProjectId} not found.");

        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate,
            ProjectId = request.ProjectId,
            AssignedToId = request.AssignedToId,
        };

        await _unitOfWork.Tasks.AddAsync(task);
        await _unitOfWork.SaveChangesAsync();

        await _cache.RemoveAsync($"tasks:project:{request.ProjectId}");

        var result = new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            DueDate = task.DueDate,
            ProjectId = task.ProjectId,
            AssignedToId = task.AssignedToId,
            CreatedAt = task.CreatedAt,
        };

        // Notify all connected clients viewing this project
        await _notifications.NotifyTaskCreated(request.ProjectId.ToString(), result);

        return result;
    }
}
