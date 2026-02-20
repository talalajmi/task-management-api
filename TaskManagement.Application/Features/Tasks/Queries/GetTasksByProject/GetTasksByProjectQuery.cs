using MediatR;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public record GetTasksByProjectQuery(Guid ProjectId) : IRequest<IEnumerable<TaskDto>>;
