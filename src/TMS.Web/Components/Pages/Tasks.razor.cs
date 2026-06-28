using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using AntDesign;
using TMS.Core.DTOs;
using TMS.Core.Enums;
using TMS.Web.Services;
using TaskStatus = TMS.Core.Enums.TaskStatus;

namespace TMS.Web.Components.Pages;

public class TaskFormModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public string SourceLanguage { get; set; } = "en";
    public string TargetLanguage { get; set; } = "ar";
    public string AssignedUserId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public int WordCount { get; set; }
    public int EstimatedHours { get; set; }
    public int EstimatedMinutes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class UserDropdownItem
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

public class UserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public partial class Tasks : ComponentBase
{
    [Inject] private ITaskWebService TaskWebService { get; set; } = default!;
    [Inject] private IUserService UserService { get; set; } = default!;
    [Inject] private MessageService Message { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private List<TaskWebDto> allTasks = new();
    private List<TaskWebDto> filteredTasks = new();
    private List<UserDto> users = new();
    private List<UserDropdownItem> userDropdownItems = new();
    private bool loading = true;
    private bool drawerVisible = false;
    private bool isEditMode = false;
    private bool submitting = false;
    private string editTaskId = string.Empty;

    private string searchText = string.Empty;
    private string selectedStatus = string.Empty;
    private string selectedUserId = string.Empty;

    private TaskFormModel formModel = new();

    private static readonly List<string> taskTypeOptions = new()
    {
        "Translation", "Review", "Writing", "Design", "Development"
    };

    private static readonly List<string> statusOptions = new()
    {
        "Pending", "Assigned", "InProgress", "Submitted", "Approved", "Rejected"
    };

    protected override async Task OnInitializedAsync()
    {
        await Task.WhenAll(LoadTasks(), LoadUsers());
    }

    private async Task LoadTasks()
    {
        loading = true;
        try
        {
            allTasks = await TaskWebService.GetAllTasksAsync();
            ApplyFilters();
        }
        catch (Exception ex)
        {
            Message.Error($"Failed to load tasks: {ex.Message}");
        }
        finally
        {
            loading = false;
        }
    }

    private async Task LoadUsers()
    {
        try
        {
            users = await UserService.GetAllUsersAsync();
            userDropdownItems = users.Select(u => new UserDropdownItem
            {
                Value = u.Id,
                Label = u.FullName
            }).ToList();

            var userJson = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user");
            if (!string.IsNullOrEmpty(userJson))
            {
                var user = System.Text.Json.JsonSerializer.Deserialize<UserInfo>(userJson);
                if (user != null && !string.IsNullOrEmpty(user.Id))
                {
                    formModel.CreatedBy = user.Id;
                }
                else
                {
                    // Use first user as default if current user not found
                    formModel.CreatedBy = users.FirstOrDefault()?.Id ?? string.Empty;
                }
            }
            else
            {
                // Use first user as default if no user in localStorage
                formModel.CreatedBy = users.FirstOrDefault()?.Id ?? string.Empty;
            }
        }
        catch { }
    }

    private void ApplyFilters()
    {
        filteredTasks = allTasks.Where(t =>
        {
            var matchesSearch = string.IsNullOrEmpty(searchText) ||
                t.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                t.AssignedUserName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                t.FileName.Contains(searchText, StringComparison.OrdinalIgnoreCase);

            var matchesStatus = string.IsNullOrEmpty(selectedStatus) ||
                t.Status.ToString() == selectedStatus;

            var matchesUser = string.IsNullOrEmpty(selectedUserId) ||
                t.AssignedUserId == selectedUserId;

            return matchesSearch && matchesStatus && matchesUser;
        }).ToList();
    }

    private void ClearFilters()
    {
        searchText = string.Empty;
        selectedStatus = string.Empty;
        selectedUserId = string.Empty;
        ApplyFilters();
    }

    private void ShowAddDrawer()
    {
        isEditMode = false;
        editTaskId = string.Empty;
        formModel = new TaskFormModel();
        drawerVisible = true;
    }

    private void OnDrawerOpen()
    {
        Console.WriteLine("[Tasks] Drawer opened");
    }

    private void ShowEditDrawer(TaskWebDto task)
    {
        isEditMode = true;
        editTaskId = task.Id;
        formModel = new TaskFormModel
        {
            Title = task.Title,
            TaskType = task.TaskType,
            AssignedUserId = task.AssignedUserId,
            FileName = task.FileName,
            FileUrl = task.FileUrl,
            FileSizeBytes = task.FileSizeBytes,
            WordCount = task.WordCount,
            EstimatedHours = task.EstimatedHours,
            EstimatedMinutes = task.EstimatedMinutes
        };
        drawerVisible = true;
    }

    private void CloseDrawer()
    {
        drawerVisible = false;
    }

    private async Task HandleSubmit()
    {
        if (string.IsNullOrWhiteSpace(formModel.Title))
        {
            Message.Warning("Task name is required.");
            return;
        }

        Console.WriteLine($"[Tasks] HandleSubmit called - Title: {formModel.Title}, FileName: {formModel.FileName}, FileUrl: {formModel.FileUrl}");

        submitting = true;
        try
        {
            var dto = new CreateTaskWebDto
            {
                Title = formModel.Title,
                Description = formModel.Description,
                TaskType = formModel.TaskType,
                SourceLanguage = formModel.SourceLanguage,
                TargetLanguage = formModel.TargetLanguage,
                AssignedUserId = formModel.AssignedUserId,
                FileName = formModel.FileName,
                FileUrl = formModel.FileUrl,
                FileSizeBytes = formModel.FileSizeBytes,
                WordCount = formModel.WordCount,
                EstimatedHours = formModel.EstimatedHours,
                EstimatedMinutes = formModel.EstimatedMinutes,
                CreatedBy = formModel.CreatedBy
            };

            Console.WriteLine($"[Tasks] Submitting task: Title={dto.Title}, File={dto.FileName}, FileUrl={dto.FileUrl}, WordCount={dto.WordCount}, CreatedBy={dto.CreatedBy}");

            if (isEditMode)
            {
                var existing = allTasks.FirstOrDefault(t => t.Id == editTaskId);
                if (existing != null)
                {
                    existing.Title = dto.Title;
                    existing.TaskType = dto.TaskType;
                    existing.AssignedUserId = dto.AssignedUserId;
                    existing.AssignedUserName = users.FirstOrDefault(u => u.Id == dto.AssignedUserId)?.FullName ?? string.Empty;
                    existing.FileName = dto.FileName;
                    existing.FileUrl = dto.FileUrl;
                    existing.FileSizeBytes = dto.FileSizeBytes;
                    existing.WordCount = dto.WordCount;
                    existing.EstimatedHours = dto.EstimatedHours;
                    existing.EstimatedMinutes = dto.EstimatedMinutes;
                }
                Console.WriteLine($"[Tasks] Task updated in memory");
                Message.Success("Task updated successfully!");
            }
            else
            {
                Console.WriteLine($"[Tasks] Creating new task via API...");
                var success = await TaskWebService.CreateTaskAsync(dto);
                if (success)
                {
                    Console.WriteLine($"[Tasks] Task created successfully, reloading tasks...");
                    await LoadTasks();
                    Message.Success("Task created successfully!");
                }
                else
                {
                    Console.WriteLine($"[Tasks] Task creation failed");
                    Message.Error("Failed to create task.");
                }
            }

            ApplyFilters();
            drawerVisible = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Tasks] Exception in HandleSubmit: {ex.Message}\n{ex.StackTrace}");
            Message.Error($"Error: {ex.Message}");
        }
        finally
        {
            submitting = false;
        }
    }

    private async Task DeleteTask(string id)
    {
        try
        {
            var success = await TaskWebService.DeleteTaskAsync(id);
            if (success)
            {
                allTasks.RemoveAll(t => t.Id == id);
                ApplyFilters();
                Message.Success("Task deleted.");
            }
        }
        catch (Exception ex)
        {
            Message.Error($"Error: {ex.Message}");
        }
    }

    private async Task HandleFileUpload(InputFileChangeEventArgs e)
    {
        var file = e.File;
        formModel.FileName = file.Name;
        formModel.FileSizeBytes = file.Size;

        Console.WriteLine($"[Tasks] File selected: {file.Name}, Size: {file.Size} bytes");

        try
        {
            using var stream = file.OpenReadStream(maxAllowedSize: 20 * 1024 * 1024);
            Console.WriteLine($"[Tasks] Uploading file to server...");
            
            var result = await TaskWebService.UploadFileAsync(stream, file.Name);
            
            if (!string.IsNullOrEmpty(result.FileUrl))
            {
                formModel.FileUrl = result.FileUrl;
                formModel.WordCount = result.WordCount;
                formModel.EstimatedHours = (int)result.EstimatedHours;
                formModel.EstimatedMinutes = (int)((result.EstimatedHours - (int)result.EstimatedHours) * 60);
                
                Console.WriteLine($"[Tasks] File uploaded successfully: FileUrl={result.FileUrl}, WordCount={result.WordCount}, EstimatedHours={result.EstimatedHours}");
                Message.Success($"File uploaded: {file.Name}");
            }
            else
            {
                Console.WriteLine($"[Tasks] File upload returned empty FileUrl, falling back to local word count");
                stream.Position = 0;
                using var reader = new StreamReader(stream);
                var content = await reader.ReadToEndAsync();
                formModel.WordCount = CountWords(content);
                Message.Warning("File uploaded but word count calculated locally");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Tasks] File upload failed: {ex.Message}");
            formModel.WordCount = EstimateWordCountFromSize(file.Size);
            Message.Error($"File upload failed: {ex.Message}");
        }
    }

    private async Task TriggerFileInput()
    {
        await JSRuntime.InvokeVoidAsync("eval", "document.getElementById('taskFileInput').click()");
    }

    private void ClearFile()
    {
        formModel.FileName = string.Empty;
        formModel.FileUrl = string.Empty;
        formModel.FileSizeBytes = 0;
        formModel.WordCount = 0;
    }

    private static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;
        return text.Split(new[] { ' ', '\n', '\r', '\t' },
            StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private static int EstimateWordCountFromSize(long bytes)
    {
        return (int)(bytes / 6);
    }

    private static string FormatFileSize(long bytes)
    {
        if (bytes >= 1_048_576) return $"{bytes / 1_048_576.0:F1} MB";
        if (bytes >= 1_024) return $"{bytes / 1_024.0:F0} KB";
        return $"{bytes} B";
    }

    private static string TruncateFileName(string name)
    {
        return name.Length > 22 ? name[..18] + "..." : name;
    }

    private static string FormatEstTime(int hours, int minutes)
    {
        if (hours == 0 && minutes == 0) return "—";
        if (hours == 0) return $"{minutes}m";
        if (minutes == 0) return $"{hours}h";
        return $"{hours}h {minutes}m";
    }

    private static string GetStatusColor(TaskStatus status) => status switch
    {
        TaskStatus.Pending => "gold",
        TaskStatus.Assigned => "blue",
        TaskStatus.InProgress => "processing",
        TaskStatus.Submitted => "purple",
        TaskStatus.Approved => "success",
        TaskStatus.Rejected => "error",
        _ => "default"
    };

    private static string GetStatusLabel(TaskStatus status) => status switch
    {
        TaskStatus.Pending => "Pending",
        TaskStatus.Assigned => "Assigned",
        TaskStatus.InProgress => "In Progress",
        TaskStatus.Submitted => "Submitted",
        TaskStatus.Approved => "Approved",
        TaskStatus.Rejected => "Rejected",
        _ => status.ToString()
    };

    private static readonly string[] AvatarColors =
    {
        "background:#3b82f6;color:white;",
        "background:#10b981;color:white;",
        "background:#f59e0b;color:white;",
        "background:#8b5cf6;color:white;",
        "background:#ef4444;color:white;",
        "background:#06b6d4;color:white;"
    };

    private static string GetAvatarStyle(string name)
    {
        var idx = Math.Abs(name.GetHashCode()) % AvatarColors.Length;
        return AvatarColors[idx];
    }
}
