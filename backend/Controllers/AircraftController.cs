using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Services;

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
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List()
    {
        var list = await _service.GetAllAsync();

        // Map to DTO to include calculated 'lastServiceDate'
        var dto = list.Select(a => new {
            a.AircraftId,
            a.Model,
            a.Category,
            a.ComplianceStatus,
            // LOGIC: Find the latest date in the history list
            lastServiceDate = a.ServiceHistory?.OrderBy(x => x.Date).LastOrDefault()?.Date
        });

        return Ok(dto);
    }

    // GET: /api/aircraft/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Aircraft), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string id)
    {
        var aircraft = await _service.GetByIdAsync(id);
        return aircraft is null ? NotFound() : Ok(aircraft);
    }

    // POST: /api/aircraft (Admin only)
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    [ProducesResponseType(typeof(Aircraft), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] Aircraft a)
    {
        var success = await _service.CreateAsync(a);

        if (!success)
            return Conflict($"Aircraft {a.AircraftId} already exists.");

        return CreatedAtAction(nameof(Get), new { id = a.AircraftId }, a);
    }

    // PUT: /api/aircraft/{id} (Admin only)
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] Aircraft patch)
    {
        var success = await _service.UpdateAsync(id, patch);
        return success ? NoContent() : NotFound();
    }

    // DELETE: /api/aircraft/{id} (Admin only)
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}