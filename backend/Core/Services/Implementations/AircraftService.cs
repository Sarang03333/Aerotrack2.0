using Microsoft.EntityFrameworkCore;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Infrastructure;

namespace AeroTrack.Api.Services;

public class AircraftService : IAircraftService
{
    private readonly AppDbContext _db;
    private readonly ILogger<AircraftService> _logger;

    public AircraftService(AppDbContext db, ILogger<AircraftService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<Aircraft>> GetAllAsync()
    {
        // FIX: Added .Include() so ServiceHistory is not null
        return await _db.Aircraft
            .Include(a => a.ServiceHistory)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Aircraft?> GetByIdAsync(string id)
    {
        return await _db.Aircraft
            .Include(a => a.ServiceHistory) // Good to have history here too
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AircraftId == id);
    }

    public async Task<bool> CreateAsync(Aircraft aircraft)
    {
        _logger.LogInformation("Registering new Aircraft: {AircraftId} ({Model})", aircraft.AircraftId, aircraft.Model);

        if (await _db.Aircraft.AnyAsync(x => x.AircraftId == aircraft.AircraftId))
        {
            _logger.LogWarning("Registration failed: Aircraft {AircraftId} already exists.", aircraft.AircraftId);
            return false;
        }

        aircraft.ComplianceStatus = "Pending";
        _db.Aircraft.Add(aircraft);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Aircraft {AircraftId} registered successfully.", aircraft.AircraftId);
        return true;
    }

    public async Task<bool> UpdateAsync(string id, Aircraft patch)
    {
        var existing = await _db.Aircraft.FindAsync(id);
        if (existing is null) 
        {
            _logger.LogWarning("Update failed: Aircraft {AircraftId} not found.", id);
            return false;
        }

        _logger.LogInformation("Updating details for Aircraft {AircraftId}", id);
        
        existing.Model = patch.Model;
        existing.Category = patch.Category;
        
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        _logger.LogWarning("Attempting to DELETE Aircraft {AircraftId}", id);

        var existing = await _db.Aircraft.FindAsync(id);
        if (existing is null) 
        {
            _logger.LogWarning("Delete failed: Aircraft {AircraftId} not found.", id);
            return false;
        }

        _db.Aircraft.Remove(existing);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Aircraft {AircraftId} deleted successfully.", id);
        return true;
    }
}