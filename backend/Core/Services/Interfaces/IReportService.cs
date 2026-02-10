namespace AeroTrack.Api.Services;

public interface IReportService
{
    Task<object> GetDashboardOverviewAsync();
    Task<byte[]> GenerateFleetReportCsvAsync();
}