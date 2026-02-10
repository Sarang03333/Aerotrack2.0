using Microsoft.EntityFrameworkCore;
using AeroTrack.Api.Infrastructure;

namespace AeroTrack.Api.Services;

public class IdGeneratorService : IIdGeneratorService
{
    private readonly AppDbContext _db;
    public IdGeneratorService(AppDbContext db) => _db = db;

    public async Task<object> GetNextAircraftIdAsync() => 
        new { id = $"AC-COM-{await GetNextNum(_db.Aircraft.Select(x=>x.AircraftId), ""):D4}" };

    public async Task<object> GetNextMaintenanceIdAsync() =>
        new { id = $"MT-{DateTime.UtcNow:yyyyMMdd}-{await GetNextNum(_db.MaintenanceTasks.Select(x=>x.TaskId), $"MT-{DateTime.UtcNow:yyyyMMdd}-"):D4}" };

    public async Task<object> GetNextAuditIdAsync() =>
        new { id = $"AU-{DateTime.UtcNow:yyyyMMdd}-{await GetNextNum(_db.AuditLogs.Select(x=>x.AuditId), $"AU-{DateTime.UtcNow:yyyyMMdd}-"):D3}" };

    public async Task<object> GetNextPartIdAsync() =>
        new { id = $"SP-GEN-{await GetNextNum(_db.SpareParts.Select(x=>x.PartId), ""):D4}" };

    private async Task<int> GetNextNum(IQueryable<string> query, string prefix)
    {
        // Simple simplified logic for demo
        var list = await query.ToListAsync(); 
        if (!string.IsNullOrEmpty(prefix)) list = list.Where(x => x.StartsWith(prefix)).ToList();
        
        if (!list.Any()) return 1;
        var max = list.Select(x => x.Split('-').Last()).Where(x => int.TryParse(x, out _)).Max(x => int.Parse(x));
        return max + 1;
    }
}