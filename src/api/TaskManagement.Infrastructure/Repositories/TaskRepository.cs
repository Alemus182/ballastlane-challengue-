using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Abstractions;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly TaskManagementDbContext _dbContext;

    public TaskRepository(TaskManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TaskItem> AddAsync(TaskItem taskItem, CancellationToken cancellationToken)
    {
        _dbContext.Tasks.Add(taskItem);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return taskItem;
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Tasks
            .Where(taskItem => taskItem.UserId == userId)
            .OrderBy(taskItem => taskItem.DueDate)
            .ToListAsync(cancellationToken);
    }

    public Task<TaskItem?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Tasks
            .FirstOrDefaultAsync(taskItem => taskItem.UserId == userId && taskItem.Id == id, cancellationToken);
    }

    public async Task<TaskItem> UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken)
    {
        _dbContext.Tasks.Update(taskItem);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return taskItem;
    }

    public async Task DeleteAsync(TaskItem taskItem, CancellationToken cancellationToken)
    {
        _dbContext.Tasks.Remove(taskItem);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}