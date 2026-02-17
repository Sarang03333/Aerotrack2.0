using System.ComponentModel.DataAnnotations;

namespace AeroTrack.Api.Core.DTOs;

public record MaintenanceTaskCreateDto(
    [Required] 
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
    
    bool IsEmergency,

    [Required]
    string Status // ADDED: Required to capture Status updates like "IN-PROGRESS"
);

public record EmergencyDto(
    [Required] string AircraftId, 
    [Required] string Description
);