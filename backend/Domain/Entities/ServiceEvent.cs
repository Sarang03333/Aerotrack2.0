namespace AeroTrack.Api.Domain.Entities;

public class ServiceEvent
{
    public int Id { get; set; }
    public string AircraftId { get; set; } = default!;
    public DateOnly Date { get; set; }

    // NEW: Link the service event to the task that completed it
    public string? TaskId { get; set; }
    public MaintenanceTask? Task { get; set; }

    public Aircraft? Aircraft { get; set; }
}