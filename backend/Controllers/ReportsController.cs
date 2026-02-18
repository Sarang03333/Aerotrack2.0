using AeroTrack.Api.Domain.Entities;
using AeroTrack.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController] [Route("api/[controller]")] [Authorize]
public class ReportsController : ControllerBase {
    private readonly IReportService _service;
    public ReportsController(IReportService s) => _service = s;

    [HttpGet("overview")] 
    public async Task<IActionResult> Overview() => Ok(await _service.GetDashboardOverviewAsync());

    [HttpGet("fleet-summary")]
    public async Task<IActionResult> Csv() {
        var bytes = await _service.GenerateFleetReportCsvAsync();
        return File(bytes, "text/csv", $"Report_{DateTime.UtcNow:yyyyMMdd}.csv");
    }
    [HttpGet("upcoming")]
public async Task<IActionResult> GetUpcoming() => Ok(await _service.GetUpcomingTasksAsync());
}