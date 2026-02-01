namespace AeroTrack.Api.Domain.Entities;

public class AuditLog
{
    public string AuditId { get; set; } = default!;
    public string AircraftId { get; set; } = default!;
    public DateOnly Date { get; set; }

    /// <summary>
    /// Findings text entered by the auditor.
    /// Example: "No discrepancies." or a description of the issue.
    /// </summary>
    public string Findings { get; set; } = default!;

    /// <summary>
    /// Severity of the audit outcome:
    /// "None" | "Minor" | "Major" | "Critical"
    /// Use "None" with Findings = "No discrepancies."
    /// </summary>
    public string Severity { get; set; } = "Minor"; // default for older records
    public Aircraft? Aircraft { get; set; }
}