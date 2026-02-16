using System.ComponentModel.DataAnnotations;

namespace AeroTrack.Api.Core.DTOs;

public record SparePartCreateDto(
    [Required]
    [RegularExpression(@"^SP-\d{3}$", ErrorMessage = "Format: SP-XXX")]
    string PartId,
    [Required] string Name,
    [Required, Range(0, 10000)] int QuantityAvailable,
    [Required, Range(0, 500)] int ReorderLevel
);