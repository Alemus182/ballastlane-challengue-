using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks;

public sealed record CreateTaskRequest(
    [property: Required, MaxLength(200)] string Title,
    string? Description,
    TaskItemStatus Status,
    DateTime? DueDate);

public sealed record UpdateTaskRequest(
    [property: Required, MaxLength(200)] string Title,
    string? Description,
    TaskItemStatus Status,
    DateTime? DueDate);

public sealed record TaskDto(
    Guid Id,
    Guid UserId,
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateTime? DueDate);