using TMS.Core.DTOs;

namespace TMS.Core.Interfaces;

public interface IReportService
{
    Task<List<ServicePerformanceDto>> GetServicePerformanceReportAsync(ReportFilterDto filter);
    Task<List<UserProductivityDto>> GetUserProductivityReportAsync(ReportFilterDto filter);
    Task<List<LateTaskDto>> GetLateTasksReportAsync(ReportFilterDto filter);
    Task<List<ProjectReportDto>> GetProjectReportAsync(ReportFilterDto filter);
}
