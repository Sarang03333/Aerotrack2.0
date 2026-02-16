using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Services;
using AeroTrack.Api.Core.DTOs; // Ensure this points to your DTO folder

namespace AeroTrack.Api.Controllers;

[ApiController] 
[Route("api/[controller]")] 
[Authorize]
public class ComplianceController : ControllerBase 
{
    // Use only one service definition to avoid ambiguity
    private readonly IComplianceService _service;
    
    public ComplianceController(IComplianceService s) => _service = s;

    [HttpGet("audits")] 
    public async Task<IActionResult> List() => Ok(await _service.GetAllAsync());

   [HttpPost("audits")]
   [Authorize(Policy = "ComplianceWrite")]
public async Task<IActionResult> Create([FromBody] AuditCreateDto dto)
{
    var audit = new AuditLog
    {
        AuditId = dto.AuditId,
        AircraftId = dto.AircraftId,
        // FIX: If Entity.Date is DateOnly, use this conversion:
        Date = DateOnly.FromDateTime(dto.Date.ToDateTime(TimeOnly.MinValue)),
        Findings = dto.Findings,
        Severity = dto.Severity
    };

    var res = await _service.CreateAsync(audit);
    return res == null 
        ? BadRequest("Invalid Aircraft or Duplicate ID") 
        : CreatedAtAction(nameof(List), new { id = audit.AuditId }, res);
}

    [HttpPut("audits/{id}")] 
    [Authorize(Policy = "ComplianceWrite")]
  public async Task<IActionResult> Update(string id, [FromBody] AuditCreateDto dto) 
{
    var auditPatch = new AuditLog 
    {
        AircraftId = dto.AircraftId,
        // FIX: Assign DateOnly directly to DateOnly
        Date = dto.Date, 
        Findings = dto.Findings,
        Severity = dto.Severity
    };

    return await _service.UpdateAsync(id, auditPatch) ? NoContent() : NotFound();
}

    [HttpDelete("audits/{id}")] 
    [Authorize(Policy = "ComplianceWrite")]
    public async Task<IActionResult> Delete(string id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}