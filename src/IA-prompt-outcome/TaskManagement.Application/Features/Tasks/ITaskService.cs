namespace TaskManagement.Application.Features.Tasks;

public interface ITaskService
{
    Task<TaskDto> CreateAsync(Guid userId, CreateTaskRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<TaskDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken);
    Task<TaskDto?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);
    Task<TaskDto?> UpdateAsync(Guid userId, Guid id, UpdateTaskRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken);
}