namespace TaskTracker.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Abstractions;
using TaskTracker.Application.DTOs.Tasks;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController(ITaskService taskService) : ControllerBase
{
    private readonly ITaskService _taskService = taskService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTasks([FromQuery] int? employeeId, [FromQuery] int? taskGroupId, [FromQuery] int? projectId, CancellationToken cancellationToken)
    {
        var tasks = await _taskService.GetAllAsync(employeeId, taskGroupId, projectId, cancellationToken);
        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTask(int id, CancellationToken cancellationToken)
    {
        var task = await _taskService.GetByIdAsync(id, cancellationToken);
        return Ok(task);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await _taskService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await _taskService.UpdateAsync(id, request, cancellationToken);
        return Ok(task);
    }

    [HttpPost("{id:int}/assignees")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddAssignee(int id, [FromBody] ModifyTaskParticipantRequest request, CancellationToken cancellationToken)
    {
        var task = await _taskService.AddAssigneeAsync(id, request, cancellationToken);
        return Ok(task);
    }

    [HttpPost("{id:int}/watchers")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddWatcher(int id, [FromBody] ModifyTaskParticipantRequest request, CancellationToken cancellationToken)
    {
        var task = await _taskService.AddWatcherAsync(id, request, cancellationToken);
        return Ok(task);
    }

    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusRequest request, CancellationToken cancellationToken)
    {
        var task = await _taskService.UpdateStatusAsync(id, request, cancellationToken);
        return Ok(task);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteTask(int id, CancellationToken cancellationToken)
    {
        await _taskService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
