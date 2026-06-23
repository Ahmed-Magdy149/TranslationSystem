using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using TMS.Core.DTOs;
using AntDesign;

namespace TMS.Web.Components.Pages;

public partial class Reports : ComponentBase
{
    private ReportFilterDto filter = new() { DateTo = DateTime.UtcNow };
    private List<ServicePerformanceDto> servicePerformanceData = new();
    private List<UserProductivityDto> userProductivityData = new();
    private List<LateTaskDto> lateTasksData = new();
    private List<ProjectReportDto> projectReportData = new();

    private bool loading = false;
    private bool exporting = false;
    private string activeTab = "service";

    private readonly List<string> serviceTypes = new()
    {
        "Translation", "Revision", "TEP", "MTPE", "QA", "LQA", "LSO", "Shooting", "Proofreading"
    };

    private readonly List<string> statusOptions = new()
    {
        "Pending", "Assigned", "InProgress", "Submitted", "Approved", "Rejected"
    };

    protected override async Task OnInitializedAsync()
    {
        await LoadReports();
    }

    private async Task LoadReports()
    {
        loading = true;
        try
        {
            await Task.WhenAll(
                LoadServicePerformance(),
                LoadUserProductivity(),
                LoadLateTasks(),
                LoadProjectReport()
            );
        }
        catch (Exception ex)
        {
            Message.Error($"Failed to load reports: {ex.Message}");
        }
        finally
        {
            loading = false;
        }
    }

    private async Task LoadServicePerformance()
    {
        try
        {
            var response = await Http.PostAsJsonAsync("/api/reports/service-performance", filter);
            if (response.IsSuccessStatusCode)
            {
                servicePerformanceData = await response.Content.ReadFromJsonAsync<List<ServicePerformanceDto>>() ?? new();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading service performance: {ex.Message}");
        }
    }

    private async Task LoadUserProductivity()
    {
        try
        {
            var response = await Http.PostAsJsonAsync("/api/reports/user-productivity", filter);
            if (response.IsSuccessStatusCode)
            {
                userProductivityData = await response.Content.ReadFromJsonAsync<List<UserProductivityDto>>() ?? new();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading user productivity: {ex.Message}");
        }
    }

    private async Task LoadLateTasks()
    {
        try
        {
            var response = await Http.PostAsJsonAsync("/api/reports/late-tasks", filter);
            if (response.IsSuccessStatusCode)
            {
                lateTasksData = await response.Content.ReadFromJsonAsync<List<LateTaskDto>>() ?? new();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading late tasks: {ex.Message}");
        }
    }

    private async Task LoadProjectReport()
    {
        try
        {
            var response = await Http.PostAsJsonAsync("/api/reports/project-report", filter);
            if (response.IsSuccessStatusCode)
            {
                projectReportData = await response.Content.ReadFromJsonAsync<List<ProjectReportDto>>() ?? new();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading project report: {ex.Message}");
        }
    }

    private void ClearFilters()
    {
        filter = new ReportFilterDto { DateTo = DateTime.UtcNow };
    }

    private void OnTabChange(string key)
    {
        activeTab = key;
    }

    private async Task ExportToExcel()
    {
        exporting = true;
        try
        {
            Message.Info("Excel export feature will be implemented soon");
            await Task.Delay(1000);
        }
        finally
        {
            exporting = false;
        }
    }

    private string GetPerformanceColor(double percentage)
    {
        if (percentage >= 90) return "success";
        if (percentage >= 70) return "processing";
        if (percentage >= 50) return "warning";
        return "error";
    }

    private string GetProgressStatus(double percentage)
    {
        if (percentage >= 90) return "success";
        if (percentage >= 50) return "active";
        return "exception";
    }

    private string FormatTimeSpan(TimeSpan span)
    {
        if (span.TotalDays >= 1)
            return $"{(int)span.TotalDays}d {span.Hours}h";
        if (span.TotalHours >= 1)
            return $"{(int)span.TotalHours}h {span.Minutes}m";
        return $"{span.Minutes}m";
    }

    private string TruncateText(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text)) return "N/A";
        return text.Length <= maxLength ? text : text[..maxLength] + "...";
    }
}
