using Microsoft.EntityFrameworkCore;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Infrastructure;

namespace AeroTrack.Api.Services;

public class MaintenanceService : IMaintenanceService
{
    private readonly AppDbContext _db;
    public MaintenanceService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<MaintenanceTask>> GetAllAsync()
    {
        return await _db.MaintenanceTasks
            .AsNoTracking()
            .OrderByDescending(t => t.IsEmergency)
            .ThenBy(t => t.Priority == "Emergency" ? 0 : 
                         t.Priority == "High" ? 1 : 
                         t.Priority == "Normal" ? 2 : 3)
            .ThenBy(t => t.ScheduledDate)
            .ThenBy(t => t.TaskId)
            .ToListAsync();
    }

    public async Task<MaintenanceTask?> GetByIdAsync(string id)
    {
        return await _db.MaintenanceTasks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TaskId == id);
    }

    // FIX: Return type matches interface (MaintenanceTask?)
    public async Task<MaintenanceTask?> CreateAsync(MaintenanceTask t)
    {
        // 1. Check if ID exists
        if (await _db.MaintenanceTasks.AnyAsync(x => x.TaskId == t.TaskId)) 
            return null; // Valid return because of '?'
        
        // 2. Normalize Priority
        t.Priority = NormalizePriority(t.Priority, t.IsEmergency);
        
        // 3. Save
        _db.MaintenanceTasks.Add(t);
        await _db.SaveChangesAsync();
        
        return t;
    }

    // FIX: Return type matches interface (MaintenanceTask?)
    public async Task<MaintenanceTask?> CreateEmergencyAsync(string aircraftId, string description)
    {
        // 1. Verify Aircraft exists
        if (!await _db.Aircraft.AnyAsync(a => a.AircraftId == aircraftId))
            return null;

        // 2. Generate ID
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"MT-{date}-";
        var last = await _db.MaintenanceTasks
            .Where(t => t.TaskId.StartsWith(prefix))
            .OrderByDescending(t => t.TaskId)
            .Select(t => t.TaskId)
            .FirstOrDefaultAsync();

        int next = 1;
        if (last != null)
        {
            var parts = last.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out var n)) 
                next = n + 1;
        }

        // 3. Create Task
        var task = new MaintenanceTask
        {
            TaskId = $"{prefix}{next:D4}",
            AircraftId = aircraftId,
            ScheduledDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = "PENDING",
            Description = description,
            IsEmergency = true,
            Priority = "Emergency"
        };

        _db.MaintenanceTasks.Add(task);
        await _db.SaveChangesAsync();
        return task;
    }

    public async Task<bool> UpdateAsync(string id, MaintenanceTask patch)
    {
        var t = await _db.MaintenanceTasks.FindAsync(id);
        if (t is null) return false;

        bool wasCompleted = t.Status == "COMPLETED";
        
        // Update fields
        t.AircraftId = patch.AircraftId;
        t.ScheduledDate = patch.ScheduledDate;
        t.Status = patch.Status;
        t.Description = patch.Description;
        t.IsEmergency = patch.IsEmergency;
        t.Priority = NormalizePriority(patch.Priority, patch.IsEmergency);

        bool isCompleted = t.Status == "COMPLETED";

        // Logic to sync ServiceEvents (History)
        if (!wasCompleted && isCompleted)
        {
             // Task finished -> Create History
             if (!await _db.ServiceEvents.AnyAsync(se => se.TaskId == t.TaskId))
             {
                _db.ServiceEvents.Add(new ServiceEvent 
                { 
                    AircraftId = t.AircraftId, 
                    Date = t.ScheduledDate, 
                    TaskId = t.TaskId 
                });
             }
        }
        else if (wasCompleted && !isCompleted)
        {
            // Task reopened -> Remove History
            var ev = await _db.ServiceEvents.FirstOrDefaultAsync(se => se.TaskId == t.TaskId);
            if (ev != null) _db.ServiceEvents.Remove(ev);
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompleteTaskAsync(string id)
    {
        var t = await _db.MaintenanceTasks.FindAsync(id);
        if (t is null) return false;

        if (t.Status != "COMPLETED")
        {
            t.Status = "COMPLETED";
            
            // Sync History
            var ev = await _db.ServiceEvents.FirstOrDefaultAsync(se => se.TaskId == t.TaskId);
            if (ev == null)
            {
                _db.ServiceEvents.Add(new ServiceEvent {
                    AircraftId = t.AircraftId,
                    Date = t.ScheduledDate == default ? DateOnly.FromDateTime(DateTime.UtcNow) : t.ScheduledDate,
                    TaskId = t.TaskId
                });
            }
            await _db.SaveChangesAsync();
        }
        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var t = await _db.MaintenanceTasks.FindAsync(id);
        if (t is null) return false;
        
        // Unlink history before delete to prevent FK errors
        var linked = await _db.ServiceEvents.Where(se => se.TaskId == id).ToListAsync();
        foreach (var se in linked) se.TaskId = null;
        
        _db.MaintenanceTasks.Remove(t);
        await _db.SaveChangesAsync();
        return true;
    }

    private static string NormalizePriority(string? incoming, bool isEmergency)
    {
        if (isEmergency) return "Emergency";
        return incoming?.Trim().ToLower() switch 
        { 
            "emergency" => "Emergency", 
            "high" => "High", 
            "low" => "Low", 
            _ => "Normal" 
        };
    }
}