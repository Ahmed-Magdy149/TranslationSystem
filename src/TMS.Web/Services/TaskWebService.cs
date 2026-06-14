using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using TMS.Core.DTOs;
using TMS.Core.Enums;
using TaskStatus = TMS.Core.Enums.TaskStatus;

namespace TMS.Web.Services;

public class TaskWebService : ITaskWebService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "/api/tasks";

    public TaskWebService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TaskWebDto>> GetAllTasksAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<TaskDto>>(BaseUrl);
            if (response == null) return new List<TaskWebDto>();

            return response.Select(t => new TaskWebDto
            {
                Id = t.Id,
                Title = t.Title,
                TaskType = "Translation",
                AssignedUserId = t.AssignedTranslatorId ?? string.Empty,
                AssignedUserName = string.Empty,
                FileName = t.FileName,
                FileUrl = t.FileUrl,
                FileSizeBytes = 0,
                WordCount = t.WordCount,
                EstimatedHours = (int)t.EstimatedHours,
                EstimatedMinutes = 0,
                Status = t.Status,
                CreatedAt = t.CreatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            return new List<TaskWebDto>();
        }
    }

    public async Task<bool> CreateTaskAsync(CreateTaskWebDto task)
    {
        try
        {
            var createDto = new CreateTaskDto
            {
                Title = task.Title,
                Description = task.Description,
                SourceLanguage = task.SourceLanguage,
                TargetLanguage = task.TargetLanguage,
                FileName = task.FileName,
                FileUrl = task.FileUrl,
                WordCount = task.WordCount,
                EstimatedHours = task.EstimatedHours + (task.EstimatedMinutes / 60.0),
                CreatedBy = task.CreatedBy
            };

            Console.WriteLine($"[TaskWebService] Creating task: {createDto.Title}, File: {createDto.FileName}, FileUrl: {createDto.FileUrl}, WordCount: {createDto.WordCount}");

            var response = await _httpClient.PostAsJsonAsync(BaseUrl, createDto);
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[TaskWebService] Task created successfully");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[TaskWebService] Failed to create task: {response.StatusCode}, {error}");
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TaskWebService] Exception creating task: {ex.Message}");
            return false;
        }
    }

    public async Task<FileUploadResult> UploadFileAsync(Stream fileStream, string fileName)
    {
        try
        {
            Console.WriteLine($"[TaskWebService] Starting file upload: {fileName}");

            var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(fileStream);
            content.Add(fileContent, "file", fileName);

            var response = await _httpClient.PostAsync($"{BaseUrl}/upload", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FileUploadResult>();
                if (result != null)
                {
                    Console.WriteLine($"[TaskWebService] File uploaded successfully: FileName={result.FileName}, FileUrl={result.FileUrl}, WordCount={result.WordCount}, EstimatedHours={result.EstimatedHours}");
                    return result;
                }
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[TaskWebService] File upload failed: {response.StatusCode}, {error}");
            }

            return new FileUploadResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TaskWebService] Exception during file upload: {ex.Message}");
            return new FileUploadResult();
        }
    }

    public async Task<bool> UpdateTaskStatusAsync(string id, TaskStatus status)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{id}/status", new { status = status.ToString() });
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteTaskAsync(string id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
