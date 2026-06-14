using Microsoft.AspNetCore.Mvc;
using TMS.Application.Interfaces;
using TMS.Core.DTOs;
using TMS.Infrastructure.Services;

namespace TMS.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IWordCounterService _wordCounterService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(
        ITaskService taskService,
        IFileStorageService fileStorageService,
        IWordCounterService wordCounterService,
        ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _fileStorageService = fileStorageService;
        _wordCounterService = wordCounterService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _taskService.GetAllAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(Core.Enums.TaskStatus status)
    {
        var tasks = await _taskService.GetByStatusAsync(status);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        _logger.LogInformation("API: Creating task - Title: {Title}, File: {FileName}, FileUrl: {FileUrl}, WordCount: {WordCount}, CreatedBy: {CreatedBy}", 
            dto.Title, dto.FileName, dto.FileUrl, dto.WordCount, dto.CreatedBy);

        var task = await _taskService.CreateAsync(dto);
        
        _logger.LogInformation("API: Task created - ID: {TaskId}, Title: {Title}, File: {FileName}, FileUrl: {FileUrl}", 
            task.Id, task.Title, task.FileName, task.FileUrl);

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPost("{id}/assign")]
    public async Task<IActionResult> Assign(string id, [FromBody] AssignTaskDto dto)
    {
        try
        {
            dto.TaskId = id;
            await _taskService.AssignTranslatorAsync(dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> Submit(string id)
    {
        try
        {
            await _taskService.SubmitTranslationAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(string id, [FromBody] ReviewRequestDto dto)
    {
        try
        {
            await _taskService.ApproveAsync(id, dto.ReviewerId, dto.Notes);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(string id, [FromBody] ReviewRequestDto dto)
    {
        try
        {
            await _taskService.RejectAsync(id, dto.ReviewerId, dto.Notes);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        _logger.LogInformation("API: File upload request received - FileName: {FileName}, Size: {Size}", 
            file?.FileName ?? "null", file?.Length ?? 0);

        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("API: File upload failed - No file provided");
            return BadRequest(new { message = "No file provided." });
        }

        var allowedExtensions = new[] { ".pdf", ".docx", ".txt" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            _logger.LogWarning("API: File upload failed - Unsupported file type: {Extension}", extension);
            return BadRequest(new { message = $"File type '{extension}' is not supported. Allowed: pdf, docx, txt." });
        }

        using var stream = file.OpenReadStream();
        var fileUrl = await _fileStorageService.UploadFileAsync(stream, file.FileName);
        _logger.LogInformation("API: File saved to storage - FileUrl: {FileUrl}", fileUrl);

        using var textStream = file.OpenReadStream();
        var text = await _fileStorageService.ExtractTextAsync(textStream, file.FileName);
        var wordCount = _wordCounterService.CountWords(text);
        var estimatedHours = _wordCounterService.EstimateHours(wordCount);

        _logger.LogInformation("API: File processed - FileName: {FileName}, FileUrl: {FileUrl}, WordCount: {WordCount}, EstimatedHours: {EstimatedHours}", 
            file.FileName, fileUrl, wordCount, estimatedHours);

        return Ok(new
        {
            FileName = file.FileName,
            FileUrl = fileUrl,
            WordCount = wordCount,
            EstimatedHours = estimatedHours
        });
    }
}

public class ReviewRequestDto
{
    public string ReviewerId { get; set; } = null!;
    public string Notes { get; set; } = string.Empty;
}
