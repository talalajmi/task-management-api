using MediatR;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public class GetTasksByProjectQueryHandler
    : IRequestHandler<GetTasksByProjectQuery, IEnumerable<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTasksByProjectQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TaskDto>> Handle(
        GetTasksByProjectQuery request,
        CancellationToken cancellationToken
    )
    {
        var tasks = await _unitOfWork.Tasks.GetByProjectIdAsync(request.ProjectId);

        return tasks.Select(task => new TaskDto
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
        });
    }
}
