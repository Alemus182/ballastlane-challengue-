using TaskManagement.Application.Abstractions;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Features.Tasks;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskDto> CreateAsync(Guid userId, CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var taskItem = new TaskItem
        {
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            DueDate = request.DueDate
        };

        var createdTask = await _taskRepository.AddAsync(taskItem, cancellationToken);
        return Map(createdTask);
    }

    public async Task<IReadOnlyList<TaskDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetAllByUserAsync(userId, cancellationToken);
        return tasks.Select(Map).ToList();
    }

    public async Task<TaskDto?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var taskItem = await _taskRepository.GetByIdAsync(userId, id, cancellationToken);
        return taskItem is null ? null : Map(taskItem);
    }

    public async Task<TaskDto?> UpdateAsync(Guid userId, Guid id, UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var taskItem = await _taskRepository.GetByIdAsync(userId, id, cancellationToken);
        if (taskItem is null)
        {
            return null;
        }

        taskItem.Title = request.Title;
        taskItem.Description = request.Description;
        taskItem.Status = request.Status;
        taskItem.DueDate = request.DueDate;

        var updatedTask = await _taskRepository.UpdateAsync(taskItem, cancellationToken);
        return Map(updatedTask);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var taskItem = await _taskRepository.GetByIdAsync(userId, id, cancellationToken);
        if (taskItem is null)
        {
            return false;
        }

        await _taskRepository.DeleteAsync(taskItem, cancellationToken);
        return true;
    }

    private static TaskDto Map(TaskItem taskItem) =>
        new(taskItem.Id, taskItem.UserId, taskItem.Title, taskItem.Description, taskItem.Status, taskItem.DueDate);
}