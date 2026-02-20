using MediatR;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> Handle(
        UpdateTaskCommand request,
        CancellationToken cancellationToken
    )
    {
        var task =
            await _unitOfWork.Tasks.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Task {request.Id} not found.");

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.AssignedToId = request.AssignedToId;

        await _unitOfWork.Tasks.UpdateAsync(task);
        await _unitOfWork.SaveChangesAsync();

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
