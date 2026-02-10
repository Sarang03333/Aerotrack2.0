using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController] [Route("api/[controller]")] [Authorize]
public class IdsController : ControllerBase {
    private readonly IIdGeneratorService _service;
    public IdsController(IIdGeneratorService s) => _service = s;

    [HttpGet("aircraft")] public async Task<IActionResult> AcId() => Ok(await _service.GetNextAircraftIdAsync());
    [HttpGet("maintenance")] public async Task<IActionResult> MtId() => Ok(await _service.GetNextMaintenanceIdAsync());
    [HttpGet("audit")] public async Task<IActionResult> AuId() => Ok(await _service.GetNextAuditIdAsync());
    [HttpGet("part")] public async Task<IActionResult> SpId() => Ok(await _service.GetNextPartIdAsync());
}