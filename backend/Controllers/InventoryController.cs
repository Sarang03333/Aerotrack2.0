using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Services;
using AeroTrack.Api.Core.DTOs;
using AutoMapper;

namespace AeroTrack.Api.Controllers;

[ApiController] 
[Route("api/[controller]")] 
[Authorize]
public class InventoryController : ControllerBase 
{
    private readonly IInventoryService _service;
    private readonly IMapper _mapper;
    
    public InventoryController(IInventoryService s, IMapper mapper) 
    {
        _service = s;
        _mapper = mapper;
    }

    [HttpGet("parts")] 
    public async Task<IActionResult> List() => Ok(await _service.GetAllAsync());

    [HttpPost("parts")] 
    [Authorize(Policy = "InventoryWrite")]
    public async Task<IActionResult> Create([FromBody] SparePartCreateDto dto) 
    {
        var part = _mapper.Map<SparePart>(dto);
        part.LastUpdated = DateOnly.FromDateTime(DateTime.Now);

        var res = await _service.CreateAsync(part);
        return res == null ? Conflict($"Part ID {dto.PartId} already exists.") : CreatedAtAction(nameof(List), new { id = part.PartId }, res);
    }

    [HttpPut("parts/{id}")] 
    [Authorize(Policy = "InventoryWrite")]
    public async Task<IActionResult> Update(string id, [FromBody] SparePartCreateDto dto) 
    {
        var partPatch = _mapper.Map<SparePart>(dto);
        partPatch.LastUpdated = DateOnly.FromDateTime(DateTime.Now);

        return await _service.UpdateAsync(id, partPatch) ? NoContent() : NotFound();
    }

    [HttpGet("parts/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var part = await _service.GetByIdAsync(id);
        return part == null ? NotFound() : Ok(part);
    }

    [HttpPut("parts/{id}/replenish")]
    public async Task<IActionResult> Replenish(string id)
    {
        var success = await _service.ReplenishAsync(id);
        return success ? NoContent() : NotFound($"Part ID {id} not found.");
    }

    [HttpDelete("parts/{id}")] 
    [Authorize(Policy = "InventoryWrite")]
    public async Task<IActionResult> Delete(string id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}