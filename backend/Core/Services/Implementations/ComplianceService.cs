using Microsoft.EntityFrameworkCore;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Infrastructure;

namespace AeroTrack.Api.Services;

public class ComplianceService : IComplianceService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ComplianceService> _logger;

    public ComplianceService(AppDbContext db, ILogger<ComplianceService> logger)
    {
        _db = db;
        _logger = logger;
    }
    public async Task<AuditLog?> GetByIdAsync(string id) => 
        await _db.AuditLogs.FindAsync(id);
    public async Task<IEnumerable<AuditLog>> GetAllAsync() => 
        await _db.AuditLogs.AsNoTracking().ToListAsync();

    public async Task<AuditLog?> CreateAsync(AuditLog a)
    {
        _logger.LogInformation("Filing Audit Report {AuditId} for Aircraft {AircraftId}", a.AuditId, a.AircraftId);

        if (!await _db.Aircraft.AnyAsync(x => x.AircraftId == a.AircraftId))
        {
            _logger.LogWarning("Audit failed: Invalid Aircraft {AircraftId}", a.AircraftId);
            return null;
        }
        
        if (await _db.AuditLogs.AnyAsync(x => x.AuditId == a.AuditId)) return null;

        a.Severity = NormalizeSeverity(a.Severity);
        a.Findings = string.IsNullOrWhiteSpace(a.Findings) ? "No discrepancies." : a.Findings.Trim();

        _db.AuditLogs.Add(a);
        await _db.SaveChangesAsync();
        
        await RecalculateCompliance(a.AircraftId);
        
        _logger.LogInformation("Audit {AuditId} filed. Severity: {Severity}", a.AuditId, a.Severity);
        return a;
    }

    public async Task<bool> UpdateAsync(string id, AuditLog patch)
    {
        var a = await _db.AuditLogs.FindAsync(id);
        if (a is null) return false;

        _logger.LogInformation("Updating Audit {AuditId}", id);

        var prevAc = a.AircraftId;
        a.AircraftId = patch.AircraftId;
        a.Date = patch.Date;
        a.Findings = patch.Findings;
        a.Severity = NormalizeSeverity(patch.Severity);

        await _db.SaveChangesAsync();

        if (prevAc != null) await RecalculateCompliance(prevAc);
        if (a.AircraftId != prevAc) await RecalculateCompliance(a.AircraftId);
        
        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var a = await _db.AuditLogs.FindAsync(id);
        if (a is null) return false;
        
        _logger.LogWarning("Deleting Audit Report {AuditId}", id);
        
        var acId = a.AircraftId;
        _db.AuditLogs.Remove(a);
        await _db.SaveChangesAsync();
        
        await RecalculateCompliance(acId);
        return true;
    }

    private async Task RecalculateCompliance(string aircraftId)
    {
        var audits = await _db.AuditLogs.Where(x => x.AircraftId == aircraftId).ToListAsync();
        string newStatus = "Compliant";
        
        if (audits.Count == 0) newStatus = "Pending";
        else if (audits.Any(a => a.Severity == "Critical")) newStatus = "Non-Compliant";
        else if (audits.Any(a => a.Severity == "Major")) newStatus = "Pending";

        var ac = await _db.Aircraft.FindAsync(aircraftId);
        if (ac != null && ac.ComplianceStatus != newStatus)
        {
            _logger.LogWarning("Compliance Status Change: Aircraft {AircraftId} is now {Status}", aircraftId, newStatus);
            ac.ComplianceStatus = newStatus;
            await _db.SaveChangesAsync();
        }
    }

    private static string NormalizeSeverity(string? s) => 
        s?.Trim().ToLower() switch { "none"=>"None", "minor"=>"Minor", "major"=>"Major", "critical"=>"Critical", _=>"Minor" };
}