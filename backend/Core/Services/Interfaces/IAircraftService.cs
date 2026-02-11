using AeroTrack.Api.Domain.Entities;

namespace AeroTrack.Api.Services;

public interface IAircraftService
{
    Task<IEnumerable<Aircraft>> GetAllAsync();
    Task<Aircraft?> GetByIdAsync(string id);
    Task<bool> CreateAsync(Aircraft aircraft);
    Task<bool> UpdateAsync(string id, Aircraft aircraft);
    Task<bool> DeleteAsync(string id);
}