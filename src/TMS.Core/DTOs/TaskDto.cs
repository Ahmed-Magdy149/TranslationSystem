namespace TMS.Core.DTOs;

public class TaskDto
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string SourceLanguage { get; set; } = null!;
    public string TargetLanguage { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public int WordCount { get; set; }
    public double EstimatedHours { get; set; }
    public string? AssignedTranslatorId { get; set; }
    public string CreatedBy { get; set; } = null!;
    public Enums.TaskStatus Status { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTaskDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string SourceLanguage { get; set; } = null!;
    public string TargetLanguage { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public int WordCount { get; set; }
    public double EstimatedHours { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime? Deadline { get; set; }
}

public class AssignTaskDto
{
    public string TaskId { get; set; } = null!;
    public string TranslatorId { get; set; } = null!;
}
