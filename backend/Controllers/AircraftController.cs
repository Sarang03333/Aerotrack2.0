using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Services;
using AeroTrack.Api.Core.DTOs;
using AeroTrack.Api.Infrastructure;

namespace AeroTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AircraftController : ControllerBase
{
    private readonly IAircraftService _service;
    private readonly AppDbContext _db;

    public AircraftController(IAircraftService service, AppDbContext db)
    {
        _service = service;
        _db = db;
    }

    [HttpGet]
    [HttpGet]
public async Task<IActionResult> List()
{
    // Explicitly include the history to avoid empty dates in the UI
    var list = await _db.Aircraft
        .Include(a => a.ServiceHistory) 
        .AsNoTracking()
        .ToListAsync();

    var result = list.Select(a => new {
        a.AircraftId,
        a.Model,
        a.Category,
        a.ComplianceStatus,
        // FIX: Cast to DateOnly? so the expression can return null if no history exists
        lastServiceDate = a.ServiceHistory != null && a.ServiceHistory.Any()
            ? (DateOnly?)a.ServiceHistory.Max(x => x.Date) 
            : (DateOnly?)null
    });

    return Ok(result);
}

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var aircraft = await _service.GetByIdAsync(id);
        return aircraft is null ? NotFound() : Ok(aircraft);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AircraftCreateDto dto)
    {
        var aircraft = new Aircraft
        {
            AircraftId = dto.AircraftId,
            Model = dto.Model,
            Category = dto.Category,
            ComplianceStatus = "Pending" // Required to prevent SQL null errors
        };

        var success = await _service.CreateAsync(aircraft);
        if (!success) return Conflict($"Aircraft {dto.AircraftId} already exists.");

        return CreatedAtAction(nameof(Get), new { id = aircraft.AircraftId }, aircraft);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] AircraftCreateDto dto)
    {
        var patch = new Aircraft
        {
            Model = dto.Model,
            Category = dto.Category
        };

        var success = await _service.UpdateAsync(id, patch);
        return success ? NoContent() : NotFound();
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}