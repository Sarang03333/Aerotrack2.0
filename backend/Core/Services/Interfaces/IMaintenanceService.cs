using AeroTrack.Api.Domain.Entities;

namespace AeroTrack.Api.Services;

public interface IMaintenanceService
{
    Task<IEnumerable<MaintenanceTask>> GetAllAsync();
    Task<MaintenanceTask?> GetByIdAsync(string id);
    Task<MaintenanceTask?> CreateAsync(MaintenanceTask task);
    Task<MaintenanceTask?> CreateEmergencyAsync(string aircraftId, string description);
    Task<bool> UpdateAsync(string id, MaintenanceTask task);
    Task<bool> CompleteTaskAsync(string id);
    Task<bool> DeleteAsync(string id);
}