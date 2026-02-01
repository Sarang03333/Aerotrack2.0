using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AeroTrack.Api.Infrastructure;

namespace AeroTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // /api/ids/*
[Authorize(Policy = "AnyRole")]
public class IdsController : ControllerBase
{
    private readonly AppDbContext _db;
    public IdsController(AppDbContext db) => _db = db;

    [HttpGet("aircraft")]
    public async Task<IActionResult> AircraftId()
    {
        int next = 1;
        var last = await _db.Aircraft.OrderByDescending(a => a.AircraftId).Select(a => a.AircraftId).FirstOrDefaultAsync();
        if (last != null && last.Split('-').Length == 3 && int.TryParse(last.Split('-')[2], out int n)) next = n + 1;
        return Ok(new { id = $"AC-COM-{next:D4}" });
    }

    [HttpGet("maintenance")]
    public async Task<IActionResult> MaintenanceId()
    {
        string date = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"MT-{date}-";
        var last = await _db.MaintenanceTasks.Where(t => t.TaskId.StartsWith(prefix))
                   .OrderByDescending(t => t.TaskId).Select(t => t.TaskId).FirstOrDefaultAsync();
        int next = 1; 
        if (last != null)
        {
            var parts = last.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int n)) next = n + 1;
        }
        return Ok(new { id = $"{prefix}{next:D4}" });
    }

    [HttpGet("audit")]
    public async Task<IActionResult> AuditId()
    {
        string date = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"AU-{date}-";
        var last = await _db.AuditLogs.Where(a => a.AuditId.StartsWith(prefix))
                   .OrderByDescending(a => a.AuditId).Select(a => a.AuditId).FirstOrDefaultAsync();
        int next = 1; 
        if (last != null)
        {
            var parts = last.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int n)) next = n + 1;
        }
        return Ok(new { id = $"{prefix}{next:D3}" });
    }

    [HttpGet("part")]
    public async Task<IActionResult> PartId()
    {
        int next = 1;
        var last = await _db.SpareParts.OrderByDescending(p => p.PartId).Select(p => p.PartId).FirstOrDefaultAsync();
        if (last != null && last.Split('-').Length == 3 && int.TryParse(last.Split('-')[2], out int n)) next = n + 1;
        return Ok(new { id = $"SP-GEN-{next:D4}" });
    }
}