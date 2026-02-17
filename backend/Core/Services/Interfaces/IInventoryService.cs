using AeroTrack.Api.Domain.Entities;

namespace AeroTrack.Api.Services;

public interface IInventoryService
{
    Task<IEnumerable<SparePart>> GetAllAsync();
    Task<SparePart?> CreateAsync(SparePart part);
    Task<bool> UpdateAsync(string id, SparePart part);
   Task<bool> ReplenishAsync(string id);// Returns result object { added, total }
    Task<bool> DeleteAsync(string id);
    Task<SparePart?> GetByIdAsync(string id);
}