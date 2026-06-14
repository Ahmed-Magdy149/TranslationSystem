using System.Net.Http.Json;
using TMS.Core.Enums;
using TaskStatus = TMS.Core.Enums.TaskStatus;

namespace TMS.Web.Services;

public class TaskWebService : ITaskWebService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "/api/tasks";

    private static readonly List<TaskWebDto> _mockTasks = new()
    {
        new TaskWebDto
        {
            Id = "1", Title = "Translate Legal Contract", TaskType = "Translation",
            AssignedUserId = "u1", AssignedUserName = "Ahmed Ali",
            FileName = "contract_2024.docx", FileSizeBytes = 245760, WordCount = 3200,
            EstimatedHours = 4, EstimatedMinutes = 30,
            Status = TaskStatus.InProgress, CreatedAt = DateTime.UtcNow.AddDays(-3)
        },
        new TaskWebDto
        {
            Id = "2", Title = "Review Marketing Brochure", TaskType = "Review",
            AssignedUserId = "u2", AssignedUserName = "Sara Mohamed",
            FileName = "brochure_v2.pdf", FileSizeBytes = 1024000, WordCount = 850,
            EstimatedHours = 1, EstimatedMinutes = 30,
            Status = TaskStatus.Pending, CreatedAt = DateTime.UtcNow.AddDays(-1)
        },
        new TaskWebDto
        {
            Id = "3", Title = "Technical Manual Translation", TaskType = "Translation",
            AssignedUserId = "u3", AssignedUserName = "Omar Hassan",
            FileName = "manual_tech.docx", FileSizeBytes = 512000, WordCount = 6500,
            EstimatedHours = 8, EstimatedMinutes = 0,
            Status = TaskStatus.Approved, CreatedAt = DateTime.UtcNow.AddDays(-7)
        },
        new TaskWebDto
        {
            Id = "4", Title = "Website Content Localization", TaskType = "Writing",
            AssignedUserId = "u1", AssignedUserName = "Ahmed Ali",
            FileName = "website_content.txt", FileSizeBytes = 38400, WordCount = 1200,
            EstimatedHours = 2, EstimatedMinutes = 0,
            Status = TaskStatus.Submitted, CreatedAt = DateTime.UtcNow.AddDays(-2)
        },
        new TaskWebDto
        {
            Id = "5", Title = "Medical Report Review", TaskType = "Review",
            AssignedUserId = "u4", AssignedUserName = "Nour Khalil",
            FileName = "medical_report.pdf", FileSizeBytes = 768000, WordCount = 2100,
            EstimatedHours = 3, EstimatedMinutes = 15,
            Status = TaskStatus.Rejected, CreatedAt = DateTime.UtcNow.AddDays(-5)
        }
    };

    public TaskWebService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TaskWebDto>> GetAllTasksAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<TaskWebDto>>(BaseUrl);
            return response ?? _mockTasks;
        }
        catch
        {
            return _mockTasks;
        }
    }

    public async Task<bool> CreateTaskAsync(CreateTaskWebDto task)
    {
        try
        {
            var newTask = new TaskWebDto
            {
                Id = Guid.NewGuid().ToString(),
                Title = task.Title,
                TaskType = task.TaskType,
                AssignedUserId = task.AssignedUserId,
                AssignedUserName = string.Empty,
                FileName = task.FileName,
                FileSizeBytes = task.FileSizeBytes,
                WordCount = task.WordCount,
                EstimatedHours = task.EstimatedHours,
                EstimatedMinutes = task.EstimatedMinutes,
                Status = TaskStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            _mockTasks.Add(newTask);
            await Task.Delay(200);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateTaskStatusAsync(string id, TaskStatus status)
    {
        var task = _mockTasks.FirstOrDefault(t => t.Id == id);
        if (task != null) task.Status = status;
        await Task.Delay(100);
        return task != null;
    }

    public async Task<bool> DeleteTaskAsync(string id)
    {
        var task = _mockTasks.FirstOrDefault(t => t.Id == id);
        if (task != null) _mockTasks.Remove(task);
        await Task.Delay(100);
        return task != null;
    }
}
