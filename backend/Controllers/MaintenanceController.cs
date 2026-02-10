using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Services;

namespace AeroTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaintenanceController : ControllerBase
{
    private readonly IMaintenanceService _service;

    public MaintenanceController(IMaintenanceService service)
    {
        _service = service;
    }

    // Define DTO here (or move to a separate file)
    public record EmergencyDto(string aircraftId, string description);

    [HttpGet("tasks")]
    public async Task<IActionResult> List()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("tasks/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var t = await _service.GetByIdAsync(id);
        return t == null ? NotFound() : Ok(t);
    }

    [HttpPost("tasks")]
    [Authorize(Policy = "MaintenanceWrite")]
    public async Task<IActionResult> Create([FromBody] MaintenanceTask t)
    {
        var res = await _service.CreateAsync(t);
        // FIX: Check for null (Duplicate ID)
        if (res == null) return Conflict($"Task {t.TaskId} already exists.");
        
        return CreatedAtAction(nameof(Get), new { id = t.TaskId }, t);
    }

    [HttpPost("tasks/emergency")]
    [Authorize(Policy = "MaintenanceWrite")]
    public async Task<IActionResult> CreateEmer([FromBody] EmergencyDto dto)
    {
        var t = await _service.CreateEmergencyAsync(dto.aircraftId, dto.description);

        // FIX: Check for null (Invalid Aircraft ID)
        if (t == null) return BadRequest($"Aircraft {dto.aircraftId} not found.");

        // Now safe to access 't.TaskId'
        return CreatedAtAction(nameof(Get), new { id = t.TaskId }, t);
    }

    [HttpPut("tasks/{id}")]
    [Authorize(Policy = "MaintenanceWrite")]
    public async Task<IActionResult> Update(string id, [FromBody] MaintenanceTask t)
    {
        var success = await _service.UpdateAsync(id, t);
        return success ? NoContent() : NotFound();
    }

    [HttpPost("tasks/{id}/complete")]
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