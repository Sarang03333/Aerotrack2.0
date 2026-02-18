using AeroTrack.Api.Core.DTOs;
using AeroTrack.Api.Domain.Entities; // Ensure this namespace is included

namespace AeroTrack.Api.Services;

public interface IReportService
{
    // FIX CS0738: Change 'object' to 'ReportDashboardDto'
    Task<ReportDashboardDto> GetDashboardOverviewAsync();
    Task<byte[]> GenerateFleetReportCsvAsync();
    Task<List<MaintenanceTask>> GetUpcomingTasksAsync();
}