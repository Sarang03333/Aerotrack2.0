namespace AeroTrack.Api.Core.DTOs;

public class ReportDashboardDto
{
    public int TotalDowntime { get; set; }
    public decimal TotalCost { get; set; }
    public int SafetyScore { get; set; }
    public int TotalAircraft { get; set; }
    public int TotalTasks { get; set; }
}