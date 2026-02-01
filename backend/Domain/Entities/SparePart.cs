namespace AeroTrack.Api.Domain.Entities;
public class SparePart
{
    public string PartId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int QuantityAvailable { get; set; }
    public int ReorderLevel { get; set; }
    public DateOnly LastUpdated { get; set; }
}
