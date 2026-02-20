using MediatR;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public class GetTasksByProjectQueryHandler(IUnitOfWork unitOfWork, ICacheService cache)
    : IRequestHandler<GetTasksByProjectQuery, IEnumerable<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<IEnumerable<TaskDto>> Handle(
        GetTasksByProjectQuery request,
        CancellationToken cancellationToken
    )
    {
        // Build a unique cache key for this specific project's tasks
        var cacheKey = $"tasks:project:{request.ProjectId}";

        // Cache-Aside Pattern:
        // Step 1: Try to get from cache first
        var cached = await _cache.GetAsync<IEnumerable<TaskDto>>(cacheKey);

        if (cached is not null)
        {
            // Cache HIT — return immediately, database never touched
            return cached;
        }

        // Cache MISS — go to database
        var tasks = await _unitOfWork.Tasks.GetByProjectIdAsync(request.ProjectId);

        var result = tasks
            .Select(task => new TaskDto
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
            })
            .ToList();

        // Store in cache for next time — expires in 10 minutes
        await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

        return result;
    }
}
