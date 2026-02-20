using MediatR;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask;

// IRequest without a type parameter means this command returns nothing
public record DeleteTaskCommand(Guid Id) : IRequest;
