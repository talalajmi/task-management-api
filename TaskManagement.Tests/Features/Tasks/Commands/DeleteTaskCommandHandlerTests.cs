using FluentAssertions;
using Moq;
using TaskManagement.Application.Features.Tasks.Commands.DeleteTask;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Tests.Features.Tasks.Commands;

public class DeleteTaskCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly DeleteTaskCommandHandler _handler;

    public DeleteTaskCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _taskRepositoryMock = new Mock<ITaskRepository>();

        _unitOfWorkMock.Setup(u => u.Tasks).Returns(_taskRepositoryMock.Object);

        _handler = new DeleteTaskCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingTask_DeletesSuccessfully()
    {
        // ARRANGE
        var taskId = Guid.NewGuid();
        var task = new TaskItem { Id = taskId, Title = "Task to delete" };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(task);

        // ACT
        await _handler.Handle(new DeleteTaskCommand(taskId), CancellationToken.None);

        // ASSERT
        _taskRepositoryMock.Verify(r => r.DeleteAsync(task), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_TaskNotFound_ThrowsKeyNotFoundException()
    {
        // ARRANGE
        _taskRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((TaskItem?)null);

        // ACT & ASSERT
        await _handler
            .Invoking(h => h.Handle(new DeleteTaskCommand(Guid.NewGuid()), CancellationToken.None))
            .Should()
            .ThrowAsync<KeyNotFoundException>();
    }
}
