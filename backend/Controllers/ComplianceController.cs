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
public class ComplianceController : ControllerBase 
{
    private readonly IComplianceService _service;
    private readonly IMapper _mapper;
    
    public ComplianceController(IComplianceService s, IMapper mapper) 
    {
        _service = s;
        _mapper = mapper;
    }

    [HttpGet("audits")] 
    public async Task<IActionResult> List() => Ok(await _service.GetAllAsync());

    [HttpPost("audits")]
    [Authorize(Policy = "ComplianceWrite")]
    public async Task<IActionResult> Create([FromBody] AuditCreateDto dto)
    {
        var audit = _mapper.Map<AuditLog>(dto);
        var res = await _service.CreateAsync(audit);
        return res == null 
            ? BadRequest("Invalid Aircraft or Duplicate ID") 
            : CreatedAtAction(nameof(List), new { id = audit.AuditId }, res);
    }

    [HttpGet("audits/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var audit = await _service.GetByIdAsync(id);
        return audit == null ? NotFound() : Ok(audit);
    }

    [HttpPut("audits/{id}")] 
    [Authorize(Policy = "ComplianceWrite")]
    public async Task<IActionResult> Update(string id, [FromBody] AuditCreateDto dto) 
    {
        var auditPatch = _mapper.Map<AuditLog>(dto);
        return await _service.UpdateAsync(id, auditPatch) ? NoContent() : NotFound();
    }

    [HttpDelete("audits/{id}")] 
    [Authorize(Policy = "ComplianceWrite")]
    public async Task<IActionResult> Delete(string id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}