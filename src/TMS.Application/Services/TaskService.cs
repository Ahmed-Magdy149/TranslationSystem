using Microsoft.Extensions.Logging;
using TMS.Application.Interfaces;
using TMS.Core.DTOs;
using TMS.Core.Entities;
using TMS.Core.Interfaces;

namespace TMS.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IRepository<TaskHistory> _taskHistoryRepository;
    private readonly IRepository<Review> _reviewRepository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(
        ITaskRepository taskRepository,
        IRepository<TaskHistory> taskHistoryRepository,
        IRepository<Review> reviewRepository,
        ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _taskHistoryRepository = taskHistoryRepository;
        _reviewRepository = reviewRepository;
        _logger = logger;
    }

    public async Task<TaskDto?> GetByIdAsync(string id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        return task == null ? null : MapToDto(task);
    }

    public async Task<IEnumerable<TaskDto>> GetAllAsync()
    {
        var tasks = await _taskRepository.GetAllAsync();
        return tasks.Select(MapToDto);
    }

    public async Task<TaskDto> CreateAsync(CreateTaskDto dto)
    {
        var task = new TranslationTask
        {
            Title = dto.Title,
            Description = dto.Description,
            SourceLanguage = dto.SourceLanguage,
            TargetLanguage = dto.TargetLanguage,
            CreatedBy = dto.CreatedBy,
            Deadline = dto.Deadline,
            Status = Core.Enums.TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _taskRepository.CreateAsync(task);

        await _taskHistoryRepository.CreateAsync(new TaskHistory
        {
            TaskId = task.Id,
            UserId = dto.CreatedBy,
            Action = "Created",
            Comment = "Task created",
            CreatedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Task created: {Title} by {CreatedBy}", task.Title, task.CreatedBy);
        return MapToDto(task);
    }

    public async Task AssignTranslatorAsync(AssignTaskDto dto)
    {
        var task = await _taskRepository.GetByIdAsync(dto.TaskId)
            ?? throw new KeyNotFoundException($"Task with id '{dto.TaskId}' not found.");

        task.AssignedTranslatorId = dto.TranslatorId;
        task.Status = Core.Enums.TaskStatus.Assigned;

        await _taskRepository.UpdateAsync(dto.TaskId, task);

        await _taskHistoryRepository.CreateAsync(new TaskHistory
        {
            TaskId = dto.TaskId,
            UserId = dto.TranslatorId,
            Action = "Assigned",
            Comment = $"Translator {dto.TranslatorId} assigned",
            CreatedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Task {TaskId} assigned to translator {TranslatorId}", dto.TaskId, dto.TranslatorId);
    }

    public async Task UpdateStatusAsync(string taskId, Core.Enums.TaskStatus status)
    {
        var task = await _taskRepository.GetByIdAsync(taskId)
            ?? throw new KeyNotFoundException($"Task with id '{taskId}' not found.");

        task.Status = status;
        await _taskRepository.UpdateAsync(taskId, task);

        await _taskHistoryRepository.CreateAsync(new TaskHistory
        {
            TaskId = taskId,
            UserId = task.CreatedBy,
            Action = $"StatusChanged:{status}",
            Comment = $"Status changed to {status}",
            CreatedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Task {TaskId} status updated to {Status}", taskId, status);
    }

    public async Task SubmitTranslationAsync(string taskId)
    {
        await UpdateStatusAsync(taskId, Core.Enums.TaskStatus.Submitted);
    }

    public async Task ApproveAsync(string taskId, string reviewerId, string notes)
    {
        await UpdateStatusAsync(taskId, Core.Enums.TaskStatus.Approved);

        await _reviewRepository.CreateAsync(new Review
        {
            TaskId = taskId,
            ReviewerId = reviewerId,
            Status = Core.Enums.ApprovalStatus.Approved,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Task {TaskId} approved by reviewer {ReviewerId}", taskId, reviewerId);
    }

    public async Task RejectAsync(string taskId, string reviewerId, string notes)
    {
        await UpdateStatusAsync(taskId, Core.Enums.TaskStatus.Rejected);

        await _reviewRepository.CreateAsync(new Review
        {
            TaskId = taskId,
            ReviewerId = reviewerId,
            Status = Core.Enums.ApprovalStatus.Rejected,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Task {TaskId} rejected by reviewer {ReviewerId}", taskId, reviewerId);
    }

    public async Task<IEnumerable<TaskDto>> GetByStatusAsync(Core.Enums.TaskStatus status)
    {
        var tasks = await _taskRepository.GetByStatusAsync(status);
        return tasks.Select(MapToDto);
    }

    public async Task<IEnumerable<TaskDto>> GetByTranslatorIdAsync(string translatorId)
    {
        var tasks = await _taskRepository.GetByTranslatorIdAsync(translatorId);
        return tasks.Select(MapToDto);
    }

    private static TaskDto MapToDto(TranslationTask task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            SourceLanguage = task.SourceLanguage,
            TargetLanguage = task.TargetLanguage,
            FileName = task.FileName,
            FileUrl = task.FileUrl,
            WordCount = task.WordCount,
            EstimatedHours = task.EstimatedHours,
            AssignedTranslatorId = task.AssignedTranslatorId,
            CreatedBy = task.CreatedBy,
            Status = task.Status,
            Deadline = task.Deadline,
            CreatedAt = task.CreatedAt
        };
    }
}
