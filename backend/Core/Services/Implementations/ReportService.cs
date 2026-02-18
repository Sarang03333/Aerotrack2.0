using Microsoft.EntityFrameworkCore;
using System.Text;
using AeroTrack.Api.Infrastructure;
using AeroTrack.Api.Core.DTOs;

namespace AeroTrack.Api.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;
    public ReportService(AppDbContext db) => _db = db;

    public async Task<ReportDashboardDto> GetDashboardOverviewAsync()
{
    var tasks = await _db.MaintenanceTasks.AsNoTracking().ToListAsync();
    var audits = await _db.AuditLogs.AsNoTracking().ToListAsync();
    var aircraftCount = await _db.Aircraft.CountAsync();

    var normal = tasks.Count(t => t.Priority != "Emergency" && t.Priority != "High");
    var heavy = tasks.Count(t => t.Priority == "Emergency" || t.Priority == "High");
    
    int safetyScore = 100;
    foreach(var a in audits) 
    {
        // Use Case-Insensitive checks to fix the 0% issue
        if (string.Equals(a.Severity, "Critical", StringComparison.OrdinalIgnoreCase)) 
            safetyScore -= 20;
        else if (string.Equals(a.Severity, "Major", StringComparison.OrdinalIgnoreCase)) 
            safetyScore -= 5;
    }

    return new ReportDashboardDto { 
        TotalDowntime = (normal * 8) + (heavy * 24),
        TotalCost = (normal * 1200) + (heavy * 5500),
        SafetyScore = Math.Max(0, safetyScore),
        TotalAircraft = aircraftCount,
        TotalTasks = tasks.Count
    };
}

    public async Task<byte[]> GenerateFleetReportCsvAsync()
    {
        var aircraft = await _db.Aircraft
            .Include(a => a.Tasks)
            .Include(a => a.Audits)
            .AsNoTracking()
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Aircraft ID,Model,Category,Tasks,Downtime,Cost,Compliance");

        foreach (var ac in aircraft)
        {
            var n = ac.Tasks.Count(t => t.Priority != "Emergency" && t.Priority != "High");
            var h = ac.Tasks.Count(t => t.Priority == "Emergency" || t.Priority == "High");
            sb.AppendLine($"{ac.AircraftId},{ac.Model},{ac.Category},{ac.Tasks.Count},{(n*8)+(h*24)},{(n*1200)+(h*5500)},{ac.ComplianceStatus}");
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}