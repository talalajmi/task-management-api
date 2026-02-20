using MediatR;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask;

// IRequestHandler<TRequest, TResponse>
// This is what MediatR looks for when CreateTaskCommand is sent
public class CreateTaskCommandHandler(IUnitOfWork unitOfWork, ICacheService cache)
    : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

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

        // Invalidate the cache for this project's tasks
        // Next GET request will fetch fresh data from database
        await _cache.RemoveAsync($"tasks:project:{request.ProjectId}");

        return new TaskDto
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
    }
}
