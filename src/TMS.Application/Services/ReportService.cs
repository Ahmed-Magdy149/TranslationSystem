using Microsoft.Extensions.Logging;
using TMS.Core.DTOs;
using TMS.Core.Entities;
using TMS.Core.Enums;
using TMS.Core.Interfaces;
using TaskStatus = TMS.Core.Enums.TaskStatus;

namespace TMS.Application.Services;

public class ReportService : IReportService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IServiceRateRepository _serviceRateRepository;
    private readonly ITaskHistoryRepository _taskHistoryRepository;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IServiceRateRepository serviceRateRepository,
        ITaskHistoryRepository taskHistoryRepository,
        ILogger<ReportService> logger)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _serviceRateRepository = serviceRateRepository;
        _taskHistoryRepository = taskHistoryRepository;
        _logger = logger;
    }

    public async Task<List<ServicePerformanceDto>> GetServicePerformanceReportAsync(ReportFilterDto filter)
    {
        var tasks = await GetFilteredTasksAsync(filter);
        
        // Only include approved tasks
        var approvedTasks = tasks.Where(t => t.Status == TaskStatus.Approved).ToList();

        var serviceGroups = approvedTasks.GroupBy(t => t.ServiceType);
        var result = new List<ServicePerformanceDto>();

        foreach (var group in serviceGroups)
        {
            var serviceType = group.Key;
            var serviceTasks = group.ToList();
            var totalWords = serviceTasks.Sum(t => t.WordCount);

            // Get service rates for this service
            var rates = await _serviceRateRepository.GetByServiceTypeAsync(serviceType);
            
            var translatorRate = rates.FirstOrDefault(r => r.Role == "Translator")?.WordsPerHour ?? 200;
            var seniorTranslatorRate = rates.FirstOrDefault(r => r.Role == "Senior Translator")?.WordsPerHour ?? 275;
            var reviewerRate = rates.FirstOrDefault(r => r.Role == "Reviewer")?.WordsPerHour ?? 300;

            var calculatedHours = totalWords / (double)translatorRate;
            
            // Calculate actual hours from completion times
            var actualHours = serviceTasks
                .Where(t => t.CompletedDate.HasValue && t.CreatedAt != null)
                .Sum(t => (t.CompletedDate!.Value - t.CreatedAt).TotalHours);

            var performancePercentage = actualHours > 0 ? (calculatedHours / actualHours) * 100 : 100;

            result.Add(new ServicePerformanceDto
            {
                ServiceType = serviceType,
                TranslatorProductivity = translatorRate,
                SeniorTranslatorProductivity = seniorTranslatorRate,
                ReviewerProductivity = reviewerRate,
                SPOC = "N/A", // Can be enhanced to track SPOC
                TotalWords = totalWords,
                CalculatedHours = Math.Round(calculatedHours, 2),
                PerformancePercentage = Math.Round(performancePercentage, 2)
            });
        }

        return result;
    }

    public async Task<List<UserProductivityDto>> GetUserProductivityReportAsync(ReportFilterDto filter)
    {
        var tasks = await GetFilteredTasksAsync(filter);
        var users = await _userRepository.GetAllAsync();
        var result = new List<UserProductivityDto>();

        foreach (var user in users)
        {
            // Get tasks assigned to this user (as translator, reviewer, etc.)
            var userTasks = tasks.Where(t => 
                t.AssignedTranslatorId == user.Id ||
                t.AssignedReviewerId == user.Id ||
                t.AssignedTeamLeaderId == user.Id ||
                t.AssignedProjectManagerId == user.Id
            ).ToList();

            if (!userTasks.Any()) continue;

            var completedTasks = userTasks.Where(t => t.Status == TaskStatus.Approved).ToList();
            var totalWords = completedTasks.Sum(t => t.WordCount);

            // Get service rate for user's role
            var serviceRate = await _serviceRateRepository.GetByServiceAndRoleAsync("Translation", user.Role.ToString());
            var wordsPerHour = serviceRate?.WordsPerHour ?? 200;

            var expectedHours = totalWords / (double)wordsPerHour;
            
            var actualHours = completedTasks
                .Where(t => t.CompletedDate.HasValue)
                .Sum(t => (t.CompletedDate!.Value - t.CreatedAt).TotalHours);

            var onTimeTasks = completedTasks.Count(t => 
                t.Deadline.HasValue && t.CompletedDate.HasValue && 
                t.CompletedDate.Value <= t.Deadline.Value);

            var lateTasks = completedTasks.Count(t => 
                t.Deadline.HasValue && t.CompletedDate.HasValue && 
                t.CompletedDate.Value > t.Deadline.Value);

            var performancePercentage = actualHours > 0 ? (expectedHours / actualHours) * 100 : 100;

            result.Add(new UserProductivityDto
            {
                UserId = user.Id,
                UserName = user.Name,
                Role = user.Role.ToString(),
                TotalTasks = userTasks.Count,
                CompletedTasks = completedTasks.Count,
                TotalWordsCompleted = totalWords,
                ExpectedHours = Math.Round(expectedHours, 2),
                ActualCompletionTime = Math.Round(actualHours, 2),
                OnTimeTasks = onTimeTasks,
                LateTasks = lateTasks,
                PerformancePercentage = Math.Round(performancePercentage, 2)
            });
        }

        return result.OrderByDescending(r => r.PerformancePercentage).ToList();
    }

    public async Task<List<LateTaskDto>> GetLateTasksReportAsync(ReportFilterDto filter)
    {
        var tasks = await GetFilteredTasksAsync(filter);
        var users = await _userRepository.GetAllAsync();
        var userDict = users.ToDictionary(u => u.Id, u => u.Name);

        var lateTasks = tasks.Where(t => 
            t.Status == TaskStatus.Approved &&
            t.Deadline.HasValue && 
            t.CompletedDate.HasValue && 
            t.CompletedDate.Value > t.Deadline.Value
        ).ToList();

        var result = new List<LateTaskDto>();

        foreach (var task in lateTasks)
        {
            // Get delay reason from task history
            var history = await _taskHistoryRepository.FindAsync(h => 
                h.TaskId == task.Id && 
                !string.IsNullOrEmpty(h.DelayReason));
            
            var delayReason = history.FirstOrDefault()?.DelayReason ?? "No reason provided";

            var userId = task.AssignedTranslatorId ?? task.AssignedReviewerId ?? task.CreatedBy;
            var userName = userDict.GetValueOrDefault(userId, "Unknown");

            result.Add(new LateTaskDto
            {
                TaskId = task.Id,
                TaskTitle = task.Title,
                UserId = userId,
                UserName = userName,
                Deadline = task.Deadline,
                CompletedDate = task.CompletedDate,
                DelayDuration = task.CompletedDate!.Value - task.Deadline!.Value,
                DelayReason = delayReason
            });
        }

        return result.OrderByDescending(r => r.DelayDuration).ToList();
    }

    public async Task<List<ProjectReportDto>> GetProjectReportAsync(ReportFilterDto filter)
    {
        var tasks = await GetFilteredTasksAsync(filter);

        var projectGroups = tasks
            .Where(t => !string.IsNullOrEmpty(t.ProjectId))
            .GroupBy(t => new { t.ProjectId, t.ProjectName });

        var result = new List<ProjectReportDto>();

        foreach (var group in projectGroups)
        {
            var projectTasks = group.ToList();
            var completedTasks = projectTasks.Count(t => t.Status == TaskStatus.Approved);
            var pendingTasks = projectTasks.Count(t => t.Status != TaskStatus.Approved);
            var totalWords = projectTasks.Sum(t => t.WordCount);
            var performancePercentage = projectTasks.Count > 0 
                ? (completedTasks / (double)projectTasks.Count) * 100 
                : 0;

            result.Add(new ProjectReportDto
            {
                ProjectId = group.Key.ProjectId!,
                ProjectName = group.Key.ProjectName ?? "Unnamed Project",
                TotalTasks = projectTasks.Count,
                CompletedTasks = completedTasks,
                PendingTasks = pendingTasks,
                TotalWords = totalWords,
                PerformancePercentage = Math.Round(performancePercentage, 2)
            });
        }

        return result.OrderByDescending(r => r.PerformancePercentage).ToList();
    }

    private async Task<List<TranslationTask>> GetFilteredTasksAsync(ReportFilterDto filter)
    {
        var tasks = await _taskRepository.GetAllAsync();

        if (filter.DateFrom.HasValue)
        {
            tasks = tasks.Where(t => t.CreatedAt >= filter.DateFrom.Value);
        }

        if (filter.DateTo.HasValue)
        {
            tasks = tasks.Where(t => t.CreatedAt <= filter.DateTo.Value);
        }

        if (!string.IsNullOrEmpty(filter.ProjectId))
        {
            tasks = tasks.Where(t => t.ProjectId == filter.ProjectId);
        }

        if (!string.IsNullOrEmpty(filter.UserId))
        {
            tasks = tasks.Where(t => 
                t.AssignedTranslatorId == filter.UserId ||
                t.AssignedReviewerId == filter.UserId ||
                t.AssignedTeamLeaderId == filter.UserId ||
                t.AssignedProjectManagerId == filter.UserId ||
                t.CreatedBy == filter.UserId);
        }

        if (!string.IsNullOrEmpty(filter.ServiceType))
        {
            tasks = tasks.Where(t => t.ServiceType == filter.ServiceType);
        }

        if (!string.IsNullOrEmpty(filter.Status))
        {
            if (Enum.TryParse<TaskStatus>(filter.Status, out var status))
            {
                tasks = tasks.Where(t => t.Status == status);
            }
        }

        return tasks.ToList();
    }
}
