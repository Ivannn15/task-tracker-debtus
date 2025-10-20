namespace TaskTracker.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Abstractions;
using TaskTracker.Application.DTOs.TaskGroups;

[ApiController]
[Route("api/task-groups")]
public sealed class TaskGroupsController(ITaskGroupService taskGroupService) : ControllerBase
{
    private readonly ITaskGroupService _taskGroupService = taskGroupService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TaskGroupDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTaskGroups(CancellationToken cancellationToken)
    {
        var groups = await _taskGroupService.GetAllAsync(cancellationToken);
        return Ok(groups);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskGroupDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTaskGroup(int id, CancellationToken cancellationToken)
    {
        var group = await _taskGroupService.GetByIdAsync(id, cancellationToken);
        return Ok(group);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskGroupDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateTaskGroup([FromBody] CreateTaskGroupRequest request, CancellationToken cancellationToken)
    {
        var group = await _taskGroupService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetTaskGroup), new { id = group.Id }, group);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TaskGroupDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTaskGroup(int id, [FromBody] UpdateTaskGroupRequest request, CancellationToken cancellationToken)
    {
        var group = await _taskGroupService.UpdateAsync(id, request, cancellationToken);
        return Ok(group);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteTaskGroup(int id, CancellationToken cancellationToken)
    {
        await _taskGroupService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
