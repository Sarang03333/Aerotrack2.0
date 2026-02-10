using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AeroTrack.Api.Infrastructure;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Services;

namespace AeroTrack.Api.Controllers;
[ApiController] [Route("api/[controller]")] [Authorize]
public class ComplianceController : ControllerBase {
    private readonly IComplianceService _service;
    public ComplianceController(IComplianceService s) => _service = s;

    [HttpGet("audits")] public async Task<IActionResult> List() => Ok(await _service.GetAllAsync());

    [HttpPost("audits")] [Authorize(Policy="ComplianceWrite")]
    public async Task<IActionResult> Create([FromBody] AuditLog a) {
        var res = await _service.CreateAsync(a);
        return res == null ? BadRequest("Invalid Aircraft or Duplicate ID") : Created("", res);
    }

    [HttpPut("audits/{id}")] [Authorize(Policy="ComplianceWrite")]
    public async Task<IActionResult> Update(string id, [FromBody] AuditLog a) =>
        await _service.UpdateAsync(id, a) ? NoContent() : NotFound();

    [HttpDelete("audits/{id}")] [Authorize(Policy="ComplianceWrite")]
    public async Task<IActionResult> Delete(string id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}