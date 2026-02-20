using MediatR;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask;

// A Command is a request to change state.
// It implements IRequest<T> where T is the return type.
// This tells MediatR "when this command is sent, return a TaskDto"
public record CreateTaskCommand : IRequest<TaskDto>
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TaskPriority Priority { get; init; } = TaskPriority.Medium;
    public DateTime? DueDate { get; init; }
    public Guid ProjectId { get; init; }
    public Guid? AssignedToId { get; init; }
}
