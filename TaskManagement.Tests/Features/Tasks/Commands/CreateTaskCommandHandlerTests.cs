using FluentAssertions;
using Moq;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Tests.Features.Tasks.Commands;

public class CreateTaskCommandHandlerTests
{
    // These are our mocked dependencies — fake versions that
    // we control completely. No real database, no real cache.
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IRepository<Project>> _projectRepositoryMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly Mock<INotificationService> _notificationMock;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _projectRepositoryMock = new Mock<IRepository<Project>>();
        _cacheMock = new Mock<ICacheService>();
        _notificationMock = new Mock<INotificationService>();

        // Wire up the mock — when UnitOfWork.Tasks is accessed,
        // return our fake task repository
        _unitOfWorkMock.Setup(u => u.Tasks).Returns(_taskRepositoryMock.Object);

        _unitOfWorkMock.Setup(u => u.Projects).Returns(_projectRepositoryMock.Object);

        // Create the handler with all mocked dependencies
        _handler = new CreateTaskCommandHandler(
            _unitOfWorkMock.Object,
            _cacheMock.Object,
            _notificationMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsTaskDto()
    {
        // ARRANGE — set up the scenario
        var projectId = Guid.NewGuid();
        var command = new CreateTaskCommand
        {
            Title = "Test Task",
            Description = "Test Description",
            Priority = TaskPriority.High,
            ProjectId = projectId,
        };

        // Tell the mock: when ExistsAsync is called with this projectId,
        // return true (project exists)
        _projectRepositoryMock.Setup(r => r.ExistsAsync(projectId)).ReturnsAsync(true);

        // ACT — execute the handler
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT — verify the result
        result.Should().NotBeNull();
        result.Title.Should().Be("Test Task");
        result.Priority.Should().Be("High");
        result.Status.Should().Be("Todo");
        result.ProjectId.Should().Be(projectId);
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ThrowsKeyNotFoundException()
    {
        // ARRANGE
        var command = new CreateTaskCommand { Title = "Test Task", ProjectId = Guid.NewGuid() };

        // Tell the mock: project does NOT exist
        _projectRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        // ACT & ASSERT — verify the exception is thrown
        await _handler
            .Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_ValidCommand_SavesTaskToRepository()
    {
        // ARRANGE
        var projectId = Guid.NewGuid();
        var command = new CreateTaskCommand { Title = "Test Task", ProjectId = projectId };

        _projectRepositoryMock.Setup(r => r.ExistsAsync(projectId)).ReturnsAsync(true);

        // ACT
        await _handler.Handle(command, CancellationToken.None);

        // ASSERT — verify AddAsync was called exactly once
        _taskRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);

        // Verify SaveChangesAsync was called exactly once
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_InvalidatesCacheForProject()
    {
        // ARRANGE
        var projectId = Guid.NewGuid();
        var command = new CreateTaskCommand { Title = "Test Task", ProjectId = projectId };

        _projectRepositoryMock.Setup(r => r.ExistsAsync(projectId)).ReturnsAsync(true);

        // ACT
        await _handler.Handle(command, CancellationToken.None);

        // ASSERT — verify cache was invalidated for this project
        _cacheMock.Verify(c => c.RemoveAsync($"tasks:project:{projectId}"), Times.Once);
    }
}
