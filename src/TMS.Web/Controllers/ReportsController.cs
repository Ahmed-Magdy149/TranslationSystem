using Microsoft.AspNetCore.Mvc;
using TMS.Core.DTOs;
using TMS.Core.Interfaces;

namespace TMS.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    [HttpPost("service-performance")]
    public async Task<IActionResult> GetServicePerformance([FromBody] ReportFilterDto filter)
    {
        try
        {
            var report = await _reportService.GetServicePerformanceReportAsync(filter);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating service performance report");
            return StatusCode(500, new { message = "Error generating report" });
        }
    }

    [HttpPost("user-productivity")]
    public async Task<IActionResult> GetUserProductivity([FromBody] ReportFilterDto filter)
    {
        try
        {
            var report = await _reportService.GetUserProductivityReportAsync(filter);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating user productivity report");
            return StatusCode(500, new { message = "Error generating report" });
        }
    }

    [HttpPost("late-tasks")]
    public async Task<IActionResult> GetLateTasks([FromBody] ReportFilterDto filter)
    {
        try
        {
            var report = await _reportService.GetLateTasksReportAsync(filter);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating late tasks report");
            return StatusCode(500, new { message = "Error generating report" });
        }
    }

    [HttpPost("project-report")]
    public async Task<IActionResult> GetProjectReport([FromBody] ReportFilterDto filter)
    {
        try
        {
            var report = await _reportService.GetProjectReportAsync(filter);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating project report");
            return StatusCode(500, new { message = "Error generating report" });
        }
    }
}
