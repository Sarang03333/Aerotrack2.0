namespace AeroTrack.Api.Domain.Entities;
public class Aircraft
{
    public string AircraftId { get; set; } = default!;
    public string Model { get; set; } = default!;
    public string Category { get; set; } = default!; // Commercial | Defense | Cargo
    public string ComplianceStatus { get; set; } = "Compliant"; // Compliant | Pending | Non-Compliant
    public ICollection<ServiceEvent> ServiceHistory { get; set; } = new List<ServiceEvent>();
    public ICollection<MaintenanceTask> Tasks { get; set; } = new List<MaintenanceTask>();
    public ICollection<AuditLog> Audits { get; set; } = new List<AuditLog>();
}
