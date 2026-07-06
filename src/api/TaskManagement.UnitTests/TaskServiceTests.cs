using TaskManagement.Application.Abstractions;
using TaskManagement.Application.Features.Tasks;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.UnitTests;

public sealed class TaskServiceTests
{
    [Fact]
    public async Task CreateAsync_PersistsTaskWithUserScope()
    {
        var repository = new InMemoryTaskRepository();
        var service = new TaskService(repository);
        var userId = Guid.NewGuid();

        var createdTask = await service.CreateAsync(
            userId,
            new CreateTaskRequest("Buy cue chalk", "Tournament prep", TaskItemStatus.Pending, DateTime.UtcNow.Date),
            CancellationToken.None);

        Assert.Equal(userId, createdTask.UserId);
        Assert.Equal("Buy cue chalk", createdTask.Title);
        Assert.Single(repository.Items);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyTasksForRequestedUser()
    {
        var repository = new InMemoryTaskRepository();
        var service = new TaskService(repository);
        var requestedUserId = Guid.NewGuid();

        await repository.AddAsync(new TaskItem { UserId = requestedUserId, Title = "Task 1" }, CancellationToken.None);
        await repository.AddAsync(new TaskItem { UserId = Guid.NewGuid(), Title = "Task 2" }, CancellationToken.None);

        var tasks = await service.GetAllAsync(requestedUserId, CancellationToken.None);

        Assert.Single(tasks);
        Assert.Equal("Task 1", tasks[0].Title);
    }

    private sealed class InMemoryTaskRepository : ITaskRepository
    {
        public List<TaskItem> Items { get; } = new();

        public Task<TaskItem> AddAsync(TaskItem taskItem, CancellationToken cancellationToken)
        {
            Items.Add(taskItem);
            return Task.FromResult(taskItem);
        }

        public Task<IReadOnlyList<TaskItem>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            IReadOnlyList<TaskItem> items = Items.Where(taskItem => taskItem.UserId == userId).ToList();
            return Task.FromResult(items);
        }

        public Task<TaskItem?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
        {
            var taskItem = Items.FirstOrDefault(item => item.UserId == userId && item.Id == id);
            return Task.FromResult(taskItem);
        }

        public Task<TaskItem> UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken)
        {
            return Task.FromResult(taskItem);
        }

        public Task DeleteAsync(TaskItem taskItem, CancellationToken cancellationToken)
        {
            Items.Remove(taskItem);
            return Task.CompletedTask;
        }
    }
}