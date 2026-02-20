namespace AeroTrack.Api.Domain.Entities;

public class AuditLog
{
    public string AuditId { get; set; } = default!;
    public string AircraftId { get; set; } = default!;
    public DateOnly Date { get; set; }
    public string Findings { get; set; } = default!;
    public string Severity { get; set; } = "Minor"; 
    public Aircraft? Aircraft { get; set; }
}