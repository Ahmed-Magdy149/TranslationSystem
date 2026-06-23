namespace TMS.Core.DTOs;

public class ServicePerformanceDto
{
    public string ServiceType { get; set; } = string.Empty;
    public int TranslatorProductivity { get; set; }
    public int SeniorTranslatorProductivity { get; set; }
    public int ReviewerProductivity { get; set; }
    public string SPOC { get; set; } = string.Empty;
    public int TotalWords { get; set; }
    public double CalculatedHours { get; set; }
    public double PerformancePercentage { get; set; }
}

public class UserProductivityDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int TotalWordsCompleted { get; set; }
    public double ExpectedHours { get; set; }
    public double ActualCompletionTime { get; set; }
    public int OnTimeTasks { get; set; }
    public int LateTasks { get; set; }
    public double PerformancePercentage { get; set; }
}

public class LateTaskDto
{
    public string TaskId { get; set; } = string.Empty;
    public string TaskTitle { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public DateTime? CompletedDate { get; set; }
    public TimeSpan DelayDuration { get; set; }
    public string DelayReason { get; set; } = string.Empty;
}

public class ProjectReportDto
{
    public string ProjectId { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public int TotalWords { get; set; }
    public double PerformancePercentage { get; set; }
}

public class ReportFilterDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; } = DateTime.UtcNow;
    public string? ProjectId { get; set; }
    public string? UserId { get; set; }
    public string? Role { get; set; }
    public string? ServiceType { get; set; }
    public string? Status { get; set; }
}

public class ServiceRateDto
{
    public string Id { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int WordsPerHour { get; set; }
}

public class CreateServiceRateDto
{
    public string ServiceType { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int WordsPerHour { get; set; }
}
