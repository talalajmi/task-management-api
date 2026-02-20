using MediatR;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task =
            await _unitOfWork.Tasks.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Task {request.Id} not found.");

        await _unitOfWork.Tasks.DeleteAsync(task);
        await _unitOfWork.SaveChangesAsync();
    }
}
