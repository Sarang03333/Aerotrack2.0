using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AeroTrack.Api.Infrastructure;
using AeroTrack.Api.Domain.Entities;

namespace AeroTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // /api/aircraft
public class AircraftController : ControllerBase
{
    private readonly AppDbContext _db;
    public AircraftController(AppDbContext db) => _db = db;

    // GET: /api/aircraft
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List()
    {
        var list = await _db.Aircraft
                            .Include(a => a.ServiceHistory)
                            .AsNoTracking()
                            .ToListAsync();

        var dto = list.Select(a => new {
            a.AircraftId,
            a.Model,
            a.Category,
            a.ComplianceStatus,
            lastServiceDate = a.ServiceHistory.OrderBy(x => x.Date).LastOrDefault()?.Date
        });

        return Ok(dto);
    }

    // GET: /api/aircraft/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Aircraft), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string id)
    {
        var a = await _db.Aircraft
                         .Include(x => x.ServiceHistory)
                         .AsNoTracking()
                         .FirstOrDefaultAsync(x => x.AircraftId == id);
        return a is null ? NotFound() : Ok(a);
    }

    // POST: /api/aircraft  (Admin only)
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    [ProducesResponseType(typeof(Aircraft), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] Aircraft a)
    {
        if (await _db.Aircraft.AnyAsync(x => x.AircraftId == a.AircraftId))
            return Conflict($"Aircraft {a.AircraftId} already exists.");

        // Server-owned compliance
        a.ComplianceStatus = "Pending";

        _db.Aircraft.Add(a);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = a.AircraftId }, a);
    }

    // PUT: /api/aircraft/{id}  (Admin only)
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] Aircraft patch)
    {
        var a = await _db.Aircraft.FindAsync(id);
        if (a is null) return NotFound();

        a.Model = patch.Model;
        a.Category = patch.Category;
        // ComplianceStatus remains server-owned via audits

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: /api/aircraft/{id}  (Admin only)
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var a = await _db.Aircraft.FindAsync(id);
        if (a is null) return NotFound();

        _db.Remove(a);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET: /api/aircraft/{id}/service-history
    [HttpGet("{id}/service-history")]
    [ProducesResponseType(typeof(IEnumerable<ServiceEvent>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> History(string id)
    {
        var exists = await _db.Aircraft.AnyAsync(a => a.AircraftId == id);
        if (!exists) return NotFound();

        var events_ = await _db.ServiceEvents
                               .Where(s => s.AircraftId == id)
                               .OrderBy(s => s.Date)
                               .AsNoTracking()
                               .ToListAsync();
        return Ok(events_);
    }
}