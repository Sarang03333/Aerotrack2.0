using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Services;
using AeroTrack.Api.Core.DTOs;

namespace AeroTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AircraftController : ControllerBase
{
    private readonly IAircraftService _service;

    public AircraftController(IAircraftService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List() => Ok(await _service.GetAllAsync());

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
        // MANUAL MAPPING: Explicitly assigning every property
        var aircraft = new Aircraft
        {
            AircraftId = dto.AircraftId,
            Model = dto.Model,
            Category = dto.Category,
            ComplianceStatus = "Pending" // Ensure a default status is set
        };

        var res = await _service.CreateAsync(aircraft);
        if (!res) return Conflict($"Aircraft {dto.AircraftId} already exists.");

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

        return await _service.UpdateAsync(id, patch) ? NoContent() : NotFound();
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}