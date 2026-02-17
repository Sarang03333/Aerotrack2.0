using Microsoft.EntityFrameworkCore;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Infrastructure;

namespace AeroTrack.Api.Services;

public class InventoryService : IInventoryService
{
    private readonly AppDbContext _db;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(AppDbContext db, ILogger<InventoryService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Fetches all spare parts from the database.
    /// Used by the Inventory List page.
    /// </summary>
    public async Task<IEnumerable<SparePart>> GetAllAsync() => 
        await _db.SpareParts.AsNoTracking().ToListAsync();

    /// <summary>
    /// Fetches a single spare part by its ID.
    /// REQUIRED to fix the empty Edit form issue.
    /// </summary>
    public async Task<SparePart?> GetByIdAsync(string id)
    {
        return await _db.SpareParts.FindAsync(id);
    }

    /// <summary>
    /// Adds a new spare part to the database.
    /// </summary>
    public async Task<SparePart?> CreateAsync(SparePart p)
    {
        if (await _db.SpareParts.AnyAsync(x => x.PartId == p.PartId)) return null;
        
        _logger.LogInformation("Added new Spare Part: {PartName} ({PartId})", p.Name, p.PartId);
        _db.SpareParts.Add(p);
        await _db.SaveChangesAsync();
        return p;
    }

    /// <summary>
    /// Updates an existing spare part record.
    /// </summary>
    public async Task<bool> UpdateAsync(string id, SparePart p)
    {
        var existing = await _db.SpareParts.FindAsync(id);
        if (existing is null) return false;
        
        existing.Name = p.Name;
        existing.QuantityAvailable = p.QuantityAvailable;
        existing.ReorderLevel = p.ReorderLevel;
        existing.LastUpdated = DateOnly.FromDateTime(DateTime.UtcNow);
        
        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Logic to automatically replenish stock based on reorder levels.
    /// </summary>
   public async Task<bool> ReplenishAsync(string id)
{
    // Assuming your DB context inside the service is named _db or _context
    var part = await _db.SpareParts.FindAsync(id);
    
    if (part == null) return false;

    // Set quantity to the standard stock level
    part.QuantityAvailable = 50; 
    part.LastUpdated = DateOnly.FromDateTime(DateTime.Now);

    await _db.SaveChangesAsync();
    return true;
}

    /// <summary>
    /// Removes a part from the database.
    /// </summary>
    public async Task<bool> DeleteAsync(string id)
    {
        var p = await _db.SpareParts.FindAsync(id);
        if (p is null) return false;
        
        _logger.LogWarning("Deleting Spare Part {PartId} from Inventory", id);
        _db.SpareParts.Remove(p);
        await _db.SaveChangesAsync();
        return true;
    }
}