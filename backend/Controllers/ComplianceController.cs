using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AeroTrack.Api.Infrastructure;
using AeroTrack.Api.Domain.Entities;

namespace AeroTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // /api/compliance
public class ComplianceController : ControllerBase
{
    private readonly AppDbContext _db;
    public ComplianceController(AppDbContext db) => _db = db;

    // GET: /api/compliance/audits
    [HttpGet("audits")]
    [ProducesResponseType(typeof(IEnumerable<AuditLog>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List() =>
        Ok(await _db.AuditLogs.AsNoTracking().ToListAsync());

    // POST: /api/compliance/audits
    [Authorize(Policy = "ComplianceWrite")]
    [HttpPost("audits")]
    [ProducesResponseType(typeof(AuditLog), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] AuditLog a)
    {
        if (!await _db.Aircraft.AnyAsync(x => x.AircraftId == a.AircraftId))
            return BadRequest($"Unknown AircraftId: {a.AircraftId}");
        if (await _db.AuditLogs.AnyAsync(x => x.AuditId == a.AuditId))
            return Conflict($"Audit {a.AuditId} exists.");

        a.Severity = NormalizeSeverity(a.Severity);
        a.Findings = string.IsNullOrWhiteSpace(a.Findings) ? "No discrepancies." : a.Findings.Trim();

        _db.AuditLogs.Add(a);
        await _db.SaveChangesAsync();

        await UpdateComplianceForAircraft(a.AircraftId);
        return Created($"api/compliance/audits/{a.AuditId}", a);
    }

    // PUT: /api/compliance/audits/{id}
    [Authorize(Policy = "ComplianceWrite")]
    [HttpPut("audits/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] AuditLog patch)
    {
        var a = await _db.AuditLogs.FindAsync(id);
        if (a is null) return NotFound();

        var prevAc = a.AircraftId;

        a.AircraftId = patch.AircraftId;
        a.Date       = patch.Date;
        a.Findings   = string.IsNullOrWhiteSpace(patch.Findings) ? "No discrepancies." : patch.Findings.Trim();
        a.Severity   = NormalizeSeverity(patch.Severity);

        await _db.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(prevAc)) await UpdateComplianceForAircraft(prevAc);
        if (!string.IsNullOrWhiteSpace(a.AircraftId) && a.AircraftId != prevAc) await UpdateComplianceForAircraft(a.AircraftId);

        return NoContent();
    }

    // DELETE: /api/compliance/audits/{id}
    [Authorize(Policy = "ComplianceWrite")]
    [HttpDelete("audits/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var a = await _db.AuditLogs.FindAsync(id);
        if (a is null) return NotFound();

        var acId = a.AircraftId;

        _db.AuditLogs.Remove(a);
        await _db.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(acId)) await UpdateComplianceForAircraft(acId);
        return NoContent();
    }

    private static string NormalizeSeverity(string? sev)
    {
        if (string.IsNullOrWhiteSpace(sev)) return "Minor";
        return sev.Trim().ToLowerInvariant() switch
        {
            "none"     => "None",
            "minor"    => "Minor",
            "major"    => "Major",
            "critical" => "Critical",
            _          => "Minor"
        };
    }

    private async Task UpdateComplianceForAircraft(string aircraftId)
    {
        var audits = await _db.AuditLogs
            .Where(x => x.AircraftId == aircraftId)
            .Select(x => new { x.Findings, x.Severity })
            .ToListAsync();

        string newStatus;
        if (audits.Count == 0) newStatus = "Pending";
        else if (audits.Any(a => a.Severity.Equals("Critical", StringComparison.OrdinalIgnoreCase))) newStatus = "Non-Compliant";
        else if (audits.Any(a => a.Severity.Equals("Major", StringComparison.OrdinalIgnoreCase) ||
                                 !string.Equals((a.Findings ?? "").Trim(), "No discrepancies.", StringComparison.OrdinalIgnoreCase)))
            newStatus = "Pending";
        else newStatus = "Compliant";

        var ac = await _db.Aircraft.FindAsync(aircraftId);
        if (ac is not null && !string.Equals(ac.ComplianceStatus, newStatus, StringComparison.Ordinal))
        {
            ac.ComplianceStatus = newStatus;
            await _db.SaveChangesAsync();
        }
    }
}