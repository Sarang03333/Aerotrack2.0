using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Services;
using AeroTrack.Api.Core.DTOs; // Required for DTO access

namespace AeroTrack.Api.Controllers;

[ApiController] 
[Route("api/[controller]")] 
[Authorize]
public class InventoryController : ControllerBase 
{
    private readonly IInventoryService _service;
    
    public InventoryController(IInventoryService s) => _service = s;

    [HttpGet("parts")] 
    public async Task<IActionResult> List() => Ok(await _service.GetAllAsync());

    [HttpPost("parts")] 
    [Authorize(Policy = "InventoryWrite")]
    public async Task<IActionResult> Create([FromBody] SparePartCreateDto dto) 
    {
        // Manual mapping from DTO to Entity to fix DateOnly conversion and ID handling
        var part = new SparePart 
        {
            PartId = dto.PartId,
            Name = dto.Name,
            QuantityAvailable = dto.QuantityAvailable,
            ReorderLevel = dto.ReorderLevel,
            // FIX CS0029: Convert current DateTime to DateOnly for the entity
            LastUpdated = DateOnly.FromDateTime(DateTime.Now)
        };

        var res = await _service.CreateAsync(part);
        return res == null ? Conflict($"Part ID {dto.PartId} already exists.") : CreatedAtAction(nameof(List), new { id = part.PartId }, res);
    }

    [HttpPut("parts/{id}")] 
    [Authorize(Policy = "InventoryWrite")]
    public async Task<IActionResult> Update(string id, [FromBody] SparePartCreateDto dto) 
    {
        var partPatch = new SparePart 
        {
            Name = dto.Name,
            QuantityAvailable = dto.QuantityAvailable,
            ReorderLevel = dto.ReorderLevel,
            LastUpdated = DateOnly.FromDateTime(DateTime.Now)
        };

        return await _service.UpdateAsync(id, partPatch) ? NoContent() : NotFound();
    }

    [HttpPost("parts/{id}/replenish")] 
    [Authorize(Policy = "InventoryWrite")]
    public async Task<IActionResult> Replenish(string id) 
    {
        var res = await _service.ReplenishAsync(id);
        return res == null ? NotFound() : Ok(res);
    }

    [HttpDelete("parts/{id}")] 
    [Authorize(Policy = "InventoryWrite")]
    public async Task<IActionResult> Delete(string id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}