using Microsoft.EntityFrameworkCore;
using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Infrastructure;

namespace AeroTrack.Api.Services;

public class InventoryService : IInventoryService
{
    private readonly AppDbContext _db;
    public InventoryService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<SparePart>> GetAllAsync() => 
        await _db.SpareParts.AsNoTracking().ToListAsync();

    public async Task<SparePart?> CreateAsync(SparePart p)
    {
        if (await _db.SpareParts.AnyAsync(x => x.PartId == p.PartId)) return null;
        _db.SpareParts.Add(p);
        await _db.SaveChangesAsync();
        return p;
    }

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

   public async Task<object?> ReplenishAsync(string id)
{
    var p = await _db.SpareParts.FindAsync(id);
    if (p is null) return null;

    var add = Math.Max(p.ReorderLevel * 2 - p.QuantityAvailable, p.ReorderLevel);
    
    p.QuantityAvailable += add;
    p.LastUpdated = DateOnly.FromDateTime(DateTime.UtcNow);
    
    await _db.SaveChangesAsync();
    return new { added = add, p.QuantityAvailable };
}

    public async Task<bool> DeleteAsync(string id)
    {
        var p = await _db.SpareParts.FindAsync(id);
        if (p is null) return false;
        _db.SpareParts.Remove(p);
        await _db.SaveChangesAsync();
        return true;
    }
}