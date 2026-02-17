using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Services;
using AeroTrack.Api.Core.DTOs;

namespace AeroTrack.Api.Controllers;

[ApiController]
[Route("api/maintenance")] 
[Authorize]
public class MaintenanceController : ControllerBase
{
    private readonly IMaintenanceService _service;

    public MaintenanceController(IMaintenanceService service)
    {
        _service = service;
    }

    [HttpGet("tasks")]
    public async Task<IActionResult> List() => Ok(await _service.GetAllAsync());

    [HttpGet("tasks/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var t = await _service.GetByIdAsync(id);
        return t == null ? NotFound() : Ok(t);
    }

    [HttpPost("tasks")]
    [Authorize(Policy = "MaintenanceWrite")]
    public async Task<IActionResult> Create([FromBody] MaintenanceTaskCreateDto dto)
    {
        var task = new MaintenanceTask
        {
            TaskId = dto.TaskId,
            AircraftId = dto.AircraftId,
            Description = dto.Description,
            Priority = dto.Priority,
            ScheduledDate = dto.ScheduledDate, 
            IsEmergency = dto.IsEmergency,
            Status = dto.Status // Maps initial status from your DTO
        };

        var res = await _service.CreateAsync(task);
        if (res == null) return Conflict($"Task {dto.TaskId} already exists or Aircraft ID is invalid.");
        
        return CreatedAtAction(nameof(Get), new { id = task.TaskId }, res);
    }

    [HttpPut("tasks/{id}")]
    [Authorize(Policy = "MaintenanceWrite")]
    public async Task<IActionResult> Update(string id, [FromBody] MaintenanceTaskCreateDto dto)
    {
        // Mapping DTO to Entity for the update logic
        var taskUpdate = new MaintenanceTask
        {
            AircraftId = dto.AircraftId,
            Description = dto.Description,
            Priority = dto.Priority,
            ScheduledDate = dto.ScheduledDate,
            IsEmergency = dto.IsEmergency,
            Status = dto.Status // FIXED: Explicitly mapping Status so "In-Progress" saves
        };

        var success = await _service.UpdateAsync(id, taskUpdate);
        return success ? NoContent() : NotFound();
    }

    [HttpPost("tasks/emergency")]
    [Authorize(Policy = "MaintenanceWrite")]
    public async Task<IActionResult> CreateEmer([FromBody] EmergencyDto dto)
    {
        var t = await _service.CreateEmergencyAsync(dto.AircraftId, dto.Description);
        if (t == null) return BadRequest($"Aircraft {dto.AircraftId} not found.");
        return CreatedAtAction(nameof(Get), new { id = t.TaskId }, t);
    }

    [HttpPut("tasks/{id}/complete")] // Change from HttpPost to HttpPut
[Authorize(Policy = "MaintenanceTransition")]
public async Task<IActionResult> Complete(string id)
{
    var success = await _service.CompleteTaskAsync(id);
    return success ? NoContent() : NotFound();
}

    [HttpDelete("tasks/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}