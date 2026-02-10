namespace AeroTrack.Api.Services;

public interface IIdGeneratorService
{
    Task<object> GetNextAircraftIdAsync();
    Task<object> GetNextMaintenanceIdAsync();
    Task<object> GetNextAuditIdAsync();
    Task<object> GetNextPartIdAsync();
}