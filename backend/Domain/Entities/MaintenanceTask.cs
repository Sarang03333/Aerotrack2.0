namespace AeroTrack.Api.Domain.Entities;

public class MaintenanceTask
{
    public string TaskId { get; set; } = default!;
    public string AircraftId { get; set; } = default!;
    public DateOnly ScheduledDate { get; set; }
    public string Status { get; set; } = "PENDING"; // PENDING | IN_PROGRESS | COMPLETED
    public string Description { get; set; } = default!;

    // NEW
    public bool IsEmergency { get; set; } = false;
    /// <summary>
    /// Priority bucket: "Emergency" | "High" | "Normal" | "Low"
    /// </summary>
    public string Priority { get; set; } = "Normal";

    public Aircraft? Aircraft { get; set; }
}