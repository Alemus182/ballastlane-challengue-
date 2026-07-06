using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Abstractions;

public interface ITaskRepository
{
    Task<TaskItem> AddAsync(TaskItem taskItem, CancellationToken cancellationToken);
    Task<IReadOnlyList<TaskItem>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<TaskItem?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);
    Task<TaskItem> UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken);
    Task DeleteAsync(TaskItem taskItem, CancellationToken cancellationToken);
}