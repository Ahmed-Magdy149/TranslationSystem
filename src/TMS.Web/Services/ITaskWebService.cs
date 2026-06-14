using TMS.Core.DTOs;
using TMS.Core.Enums;
using TaskStatus = TMS.Core.Enums.TaskStatus;

namespace TMS.Web.Services;

public class TaskWebDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public string AssignedUserId { get; set; } = string.Empty;
    public string AssignedUserName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public int WordCount { get; set; }
    public int EstimatedHours { get; set; }
    public int EstimatedMinutes { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTaskWebDto
{
    public string Title { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public string AssignedUserId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public int WordCount { get; set; }
    public int EstimatedHours { get; set; }
    public int EstimatedMinutes { get; set; }
}

public interface ITaskWebService
{
    Task<List<TaskWebDto>> GetAllTasksAsync();
    Task<bool> CreateTaskAsync(CreateTaskWebDto task);
    Task<bool> UpdateTaskStatusAsync(string id, TaskStatus status);
    Task<bool> DeleteTaskAsync(string id);
}
