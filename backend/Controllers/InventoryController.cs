using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AeroTrack.Api.Infrastructure;
using AeroTrack.Api.Domain.Entities;

namespace AeroTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _db;
    public InventoryController(AppDbContext db) => _db = db;

    // GET: /api/inventory/parts
    [HttpGet("parts")]
    [ProducesResponseType(typeof(IEnumerable<SparePart>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Parts() =>
        Ok(await _db.SpareParts.AsNoTracking().ToListAsync());

    // POST: /api/inventory/parts
    [Authorize(Policy = "InventoryWrite")]
    [HttpPost("parts")]
    [ProducesResponseType(typeof(SparePart), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] SparePart p)
    {
        if (await _db.SpareParts.AnyAsync(x => x.PartId == p.PartId))
            return Conflict($"Part {p.PartId} exists.");
        _db.SpareParts.Add(p);
        await _db.SaveChangesAsync();
        return Created($"api/inventory/parts/{p.PartId}", p);
    }

    // PUT: /api/inventory/parts/{id}
    [Authorize(Policy = "InventoryWrite")]
    [HttpPut("parts/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] SparePart patch)
    {
        var p = await _db.SpareParts.FindAsync(id);
        if (p is null) return NotFound();
        p.Name = patch.Name;
        p.QuantityAvailable = patch.QuantityAvailable;
        p.ReorderLevel = patch.ReorderLevel;
        p.LastUpdated = DateOnly.FromDateTime(DateTime.UtcNow);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // POST: /api/inventory/parts/{id}/replenish
    [Authorize(Policy = "InventoryWrite")]
    [HttpPost("parts/{id}/replenish")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Replenish(string id)
    {
        var p = await _db.SpareParts.FindAsync(id);
        if (p is null) return NotFound();

        var add = Math.Max(p.ReorderLevel * 2 - p.QuantityAvailable, p.ReorderLevel);
        p.QuantityAvailable += add;
        p.LastUpdated = DateOnly.FromDateTime(DateTime.UtcNow);
        await _db.SaveChangesAsync();

        return Ok(new { added = add, p.QuantityAvailable });
    }

    // DELETE: /api/inventory/parts/{id}
    [Authorize(Policy = "InventoryWrite")]
    [HttpDelete("parts/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var p = await _db.SpareParts.FindAsync(id);
        if (p is null) return NotFound();
        _db.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}