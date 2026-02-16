using System.ComponentModel.DataAnnotations;

namespace AeroTrack.Api.Core.DTOs;

public record MaintenanceTaskCreateDto(
    [Required] 
    // Updated Regex: Matches MT- followed by exactly 4 digits, a hyphen, and 3 digits
    // This will ACCEPT: MT-2026-006
    // This will REJECT: MT-252-876
    [RegularExpression(@"^MT-\d{4}-\d{3}$", ErrorMessage = "Task ID must follow the format MT-YYYY-XXX (e.g., MT-2026-001)")] 
    string TaskId, 
    
    [Required] 
    string AircraftId, 
    
    [Required, MinLength(10, ErrorMessage = "Description must be at least 10 characters long")] 
    string Description, 
    
    [Required] 
    string Priority, 
    
    [Required] 
    DateOnly ScheduledDate, 
    
    bool IsEmergency
);

public record EmergencyDto(
    [Required] string AircraftId, 
    [Required] string Description
);