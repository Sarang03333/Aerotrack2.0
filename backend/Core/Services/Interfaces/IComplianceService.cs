using AeroTrack.Api.Domain.Entities;

namespace AeroTrack.Api.Services;

public interface IComplianceService
{
    Task<IEnumerable<AuditLog>> GetAllAsync();
    Task<AuditLog?> CreateAsync(AuditLog audit);
    Task<bool> UpdateAsync(string id, AuditLog audit);
    Task<bool> DeleteAsync(string id);
}