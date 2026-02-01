using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AeroTrack.Api.Infrastructure;
using AeroTrack.Api.Domain.Entities;

namespace AeroTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // /api/maintenance
public class MaintenanceController : ControllerBase
{
    private readonly AppDbContext _db;
    public MaintenanceController(AppDbContext db) => _db = db;

    // ---------- LIST (emergency first, then by priority, then by date/id) ----------
    [HttpGet("tasks")]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceTask>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List()
    {
        var data = await _db.MaintenanceTasks
            .AsNoTracking()
            .OrderByDescending(t => t.IsEmergency)
            .ThenBy(t => t.Priority == "Emergency" ? 0 :
                         t.Priority == "High"      ? 1 :
                         t.Priority == "Normal"    ? 2 : 3)
            .ThenBy(t => t.ScheduledDate)
            .ThenBy(t => t.TaskId)
            .ToListAsync();

        return Ok(data);
    }

    // GET: /api/maintenance/tasks/{id}
    [HttpGet("tasks/{id}")]
    [ProducesResponseType(typeof(MaintenanceTask), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTask(string id)
    {
        var t = await _db.MaintenanceTasks.AsNoTracking().FirstOrDefaultAsync(x => x.TaskId == id);
        return t is null ? NotFound() : Ok(t);
    }

    // ---------- CREATE ----------
    [Authorize(Policy = "MaintenanceWrite")]
    [HttpPost("tasks")]
    [ProducesResponseType(typeof(MaintenanceTask), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] MaintenanceTask t)
    {
        if (!await _db.Aircraft.AnyAsync(a => a.AircraftId == t.AircraftId))
            return BadRequest($"Unknown AircraftId: {t.AircraftId}");
        if (await _db.MaintenanceTasks.AnyAsync(x => x.TaskId == t.TaskId))
            return Conflict($"Task {t.TaskId} exists.");

        t.Priority = NormalizePriority(t.Priority, t.IsEmergency);
        _db.MaintenanceTasks.Add(t);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = t.TaskId }, t);
    }

    // ---------- QUICK CREATE EMERGENCY ----------
    public record EmergencyCreateDto(string aircraftId, string description);

    [Authorize(Policy = "MaintenanceWrite")]
    [HttpPost("tasks/emergency")]
    [ProducesResponseType(typeof(MaintenanceTask), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEmergency([FromBody] EmergencyCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.aircraftId) || string.IsNullOrWhiteSpace(dto.description))
            return BadRequest("aircraftId and description are required.");
        if (!await _db.Aircraft.AnyAsync(a => a.AircraftId == dto.aircraftId))
            return BadRequest($"Unknown AircraftId: {dto.aircraftId}");

        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"MT-{date}-";
        var last = await _db.MaintenanceTasks
            .Where(t => t.TaskId.StartsWith(prefix))
            .OrderByDescending(t => t.TaskId)
            .Select(t => t.TaskId)
            .FirstOrDefaultAsync();

        int next = 1;
        if (last != null)
        {
            var parts = last.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out var n)) next = n + 1;
        }

        var task = new MaintenanceTask
        {
            TaskId = $"{prefix}{next:D4}",
            AircraftId = dto.aircraftId,
            ScheduledDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = "PENDING",
            Description = dto.description.Trim(),
            IsEmergency = true,
            Priority = "Emergency"
        };

        _db.MaintenanceTasks.Add(task);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = task.TaskId }, task);
    }

    // ---------- UPDATE ----------
    [Authorize(Policy = "MaintenanceWrite")]
    [HttpPut("tasks/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] MaintenanceTask patch)
    {
        var t = await _db.MaintenanceTasks.FindAsync(id);
        if (t is null) return NotFound();

        var wasCompleted = t.Status == "COMPLETED";

        t.AircraftId    = patch.AircraftId;
        t.ScheduledDate = patch.ScheduledDate;
        t.Status        = patch.Status;
        t.Description   = patch.Description;
        t.IsEmergency   = patch.IsEmergency;
        t.Priority      = NormalizePriority(patch.Priority, patch.IsEmergency);

        var willBeCompleted = t.Status == "COMPLETED";

        if (!wasCompleted && willBeCompleted)
        {
            var exists = await _db.ServiceEvents.AnyAsync(se => se.TaskId == t.TaskId);
            if (!exists)
                _db.ServiceEvents.Add(new ServiceEvent { AircraftId = t.AircraftId, Date = t.ScheduledDate, TaskId = t.TaskId });
        }
        else if (wasCompleted && !willBeCompleted)
        {
            var ev = await _db.ServiceEvents.FirstOrDefaultAsync(se => se.TaskId == t.TaskId);
            if (ev != null) _db.ServiceEvents.Remove(ev);
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ---------- COMPLETE ----------
    [Authorize(Policy = "MaintenanceTransition")]
    [HttpPost("tasks/{id}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(string id)
    {
        var t = await _db.MaintenanceTasks.FindAsync(id);
        if (t is null) return NotFound();

        if (t.Status != "COMPLETED")
        {
            t.Status = "COMPLETED";
            await _db.SaveChangesAsync();

            var ev = await _db.ServiceEvents.FirstOrDefaultAsync(se => se.TaskId == t.TaskId);
            if (ev == null)
            {
                _db.ServiceEvents.Add(new ServiceEvent {
                    AircraftId = t.AircraftId,
                    Date = t.ScheduledDate == default ? DateOnly.FromDateTime(DateTime.UtcNow) : t.ScheduledDate,
                    TaskId = t.TaskId
                });
            }
            else
            {
                ev.Date = t.ScheduledDate == default ? DateOnly.FromDateTime(DateTime.UtcNow) : t.ScheduledDate;
            }
            await _db.SaveChangesAsync();
        }
        return NoContent();
    }

    // ---------- DELETE (Admin only) ----------
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("tasks/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var t = await _db.MaintenanceTasks.FindAsync(id);
        if (t is null) return NotFound();

        // FK (NO ACTION) – unlink before delete to keep history
        var linked = await _db.ServiceEvents.Where(se => se.TaskId == id).ToListAsync();
        if (linked.Count > 0)
        {
            foreach (var se in linked) se.TaskId = null;
            await _db.SaveChangesAsync();
        }

        _db.Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static string NormalizePriority(string? incoming, bool isEmergency)
    {
        if (isEmergency) return "Emergency";
        if (string.IsNullOrWhiteSpace(incoming)) return "Normal";
        return incoming.Trim().ToLowerInvariant() switch
        {
            "emergency" => "Emergency",
            "high"      => "High",
            "low"       => "Low",
            _           => "Normal"
        };
    }
}