using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Tasks;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await _taskService.CreateAsync(GetCurrentUserId(), request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetAll(CancellationToken cancellationToken)
    {
        var tasks = await _taskService.GetAllAsync(GetCurrentUserId(), cancellationToken);
        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var task = await _taskService.GetByIdAsync(GetCurrentUserId(), id, cancellationToken);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskDto>> Update(Guid id, [FromBody] UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await _taskService.UpdateAsync(GetCurrentUserId(), id, request, cancellationToken);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _taskService.DeleteAsync(GetCurrentUserId(), id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private Guid GetCurrentUserId()
    {
        var rawUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(rawUserId, out var userId) ? userId : Guid.Empty;
    }
}