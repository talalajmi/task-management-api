using MediatR;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTaskById;

// A Query never changes state — it only reads
public record GetTaskByIdQuery(Guid Id) : IRequest<TaskDto>;
