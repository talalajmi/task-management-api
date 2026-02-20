using FluentAssertions;
using Moq;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Tests.Features.Tasks.Queries;

public class GetTasksByProjectQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly GetTasksByProjectQueryHandler _handler;

    public GetTasksByProjectQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _cacheMock = new Mock<ICacheService>();

        _unitOfWorkMock.Setup(u => u.Tasks).Returns(_taskRepositoryMock.Object);

        _handler = new GetTasksByProjectQueryHandler(_unitOfWorkMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_CacheHit_ReturnsCachedData()
    {
        // ARRANGE
        var projectId = Guid.NewGuid();
        var cachedTasks = new List<TaskDto>
        {
            new TaskDto { Id = Guid.NewGuid(), Title = "Cached Task" },
        };

        // Tell the mock: cache has data for this key
        _cacheMock
            .Setup(c => c.GetAsync<IEnumerable<TaskDto>>($"tasks:project:{projectId}"))
            .ReturnsAsync(cachedTasks);

        // ACT
        var result = await _handler.Handle(
            new GetTasksByProjectQuery(projectId),
            CancellationToken.None
        );

        // ASSERT — returned cached data
        result.Should().BeEquivalentTo(cachedTasks);

        // Critical: verify database was NEVER touched
        _taskRepositoryMock.Verify(r => r.GetByProjectIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CacheMiss_QueriesDatabaseAndCachesResult()
    {
        // ARRANGE
        var projectId = Guid.NewGuid();
        var dbTasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "DB Task",
                ProjectId = projectId,
            },
        };

        // Cache returns null — cache miss
        _cacheMock
            .Setup(c => c.GetAsync<IEnumerable<TaskDto>>(It.IsAny<string>()))
            .ReturnsAsync((IEnumerable<TaskDto>?)null);

        // Database returns tasks
        _taskRepositoryMock.Setup(r => r.GetByProjectIdAsync(projectId)).ReturnsAsync(dbTasks);

        // ACT
        var result = await _handler.Handle(
            new GetTasksByProjectQuery(projectId),
            CancellationToken.None
        );

        // ASSERT
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("DB Task");

        // Verify result was stored in cache
        _cacheMock.Verify(
            c =>
                c.SetAsync(
                    $"tasks:project:{projectId}",
                    It.IsAny<IEnumerable<TaskDto>>(),
                    It.IsAny<TimeSpan?>()
                ),
            Times.Once
        );
    }
}
