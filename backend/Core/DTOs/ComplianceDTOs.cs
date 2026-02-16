using System.ComponentModel.DataAnnotations;

namespace AeroTrack.Api.Core.DTOs;

public record AuditCreateDto(
    [Required] 
    [RegularExpression(@"^AUD-\d{3}$", ErrorMessage = "Format must be AUD-XXX")] 
    string AuditId,
    [Required] string AircraftId,
    [Required] DateOnly Date, 
    [Required] string Findings,
    [Required] string Severity
);