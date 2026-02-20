using MediatR;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTaskByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task =
            await _unitOfWork.Tasks.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Task {request.Id} not found.");

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
            AssignedToName = task.AssignedTo?.FullName,
            CreatedAt = task.CreatedAt,
        };
    }
}
