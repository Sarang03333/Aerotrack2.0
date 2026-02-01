using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using AeroTrack.Api.Infrastructure;

namespace AeroTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ReportsController(AppDbContext db) => _db = db;

    // 1. GET: Executive Summary Data (For UI Display)
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
    {
        var tasks = await _db.MaintenanceTasks.AsNoTracking().ToListAsync();
        var aircraft = await _db.Aircraft.AsNoTracking().ToListAsync();
        var audits = await _db.AuditLogs.AsNoTracking().ToListAsync();

        // --- FORMULAE ---

        // 1. Downtime Hours:
        // Formula: (Normal Tasks * 8 hours) + (Emergency/High Tasks * 24 hours)
        var normalCount = tasks.Count(t => t.Priority != "Emergency" && t.Priority != "High");
        var heavyCount = tasks.Count(t => t.Priority == "Emergency" || t.Priority == "High");
        var totalDowntime = (normalCount * 8) + (heavyCount * 24);

        // 2. Maintenance Cost:
        // Formula: (Normal Tasks * $1,200) + (Emergency Tasks * $5,500)
        // Note: Includes estimated labor + parts average
        var totalCost = (normalCount * 1200) + (heavyCount * 5500);

        // 3. Safety Performance (0-100 Score):
        // Formula: Start at 100. Deduct 20 for 'Critical' audits, 5 for 'Major' findings.
        // Cap at 0 (cannot be negative).
        int safetyScore = 100;
        foreach(var a in audits) 
        {
            if (a.Severity == "Critical") safetyScore -= 20;
            else if (a.Severity == "Major") safetyScore -= 5;
        }
        if (safetyScore < 0) safetyScore = 0;

        return Ok(new { 
            totalDowntime, 
            totalCost, 
            safetyScore,
            totalAircraft = aircraft.Count,
            totalTasks = tasks.Count
        });
    }

    // 2. GET: CSV Download (Detailed Report)
    [HttpGet("fleet-summary")]
    public async Task<IActionResult> GenerateFleetReport()
    {
        var aircraft = await _db.Aircraft
            .Include(a => a.Tasks)
            .Include(a => a.Audits)
            .AsNoTracking()
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Aircraft ID,Model,Category,Total Tasks,High Priority Events,Downtime (Hrs),Maint Cost ($),Compliance,Safety Risk");

        foreach (var ac in aircraft)
        {
            // Apply same formulae per aircraft
            var n = ac.Tasks.Count(t => t.Priority != "Emergency" && t.Priority != "High");
            var h = ac.Tasks.Count(t => t.Priority == "Emergency" || t.Priority == "High");
            
            var downtime = (n * 8) + (h * 24);
            var cost = (n * 1200) + (h * 5500);
            
            // Safety Risk Level
            var criticals = ac.Audits.Count(a => a.Severity == "Critical");
            var risk = criticals > 0 ? "HIGH" : (ac.ComplianceStatus == "Non-Compliant" ? "Medium" : "Low");

            sb.AppendLine($"{ac.AircraftId},{ac.Model},{ac.Category},{ac.Tasks.Count},{h},{downtime},{cost},{ac.ComplianceStatus},{risk}");
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/csv", $"AeroTrack_Fleet_Report_{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}