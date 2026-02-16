using System.ComponentModel.DataAnnotations;

namespace AeroTrack.Api.Core.DTOs;

public record AircraftCreateDto(
    [Required]
    [RegularExpression(@"^AC-[A-Z]{3}-\d{3}$", ErrorMessage = "Format must be AC-XXX-000 (e.g., AC-COM-001)")]
    string AircraftId,

    [Required]
    string Model, // Matches "Model" input in UI

    [Required]
    string Category // Matches "Category" dropdown in UI
);