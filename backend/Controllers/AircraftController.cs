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

    public AircraftController(IAircraftService service)
    {
        _service = service;
    }

    // GET: /api/aircraft
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var list = await _service.GetAllAsync();

        // Map to anonymous DTO for the list view to include calculated 'lastServiceDate'
        var result = list.Select(a => new {
            a.AircraftId,
            a.Model,
            a.Category,
            a.ComplianceStatus,
            lastServiceDate = a.ServiceHistory?.OrderBy(x => x.Date).LastOrDefault()?.Date
        });

        return Ok(result);
    }

    // GET: /api/aircraft/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var aircraft = await _service.GetByIdAsync(id);
        return aircraft is null ? NotFound() : Ok(aircraft);
    }

    // POST: /api/aircraft (Admin only)
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AircraftCreateDto dto)
    {
        // Explicitly map DTO to Entity
        var aircraft = new Aircraft
        {
            AircraftId = dto.AircraftId,
            Model = dto.Model,
            Category = dto.Category,
        };

        var success = await _service.CreateAsync(aircraft);

        if (!success)
            return Conflict($"Aircraft {dto.AircraftId} already exists.");

        return CreatedAtAction(nameof(Get), new { id = aircraft.AircraftId }, aircraft);
    }

    // PUT: /api/aircraft/{id} (Admin only)
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] AircraftCreateDto dto)
    {
        var patch = new Aircraft
        {
            Model = dto.Model,
            Category = dto.Category,
        };

        var success = await _service.UpdateAsync(id, patch);
        return success ? NoContent() : NotFound();
    }

    // DELETE: /api/aircraft/{id} (Admin only)
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}