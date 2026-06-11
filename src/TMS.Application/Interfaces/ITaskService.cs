using TMS.Core.DTOs;

namespace TMS.Application.Interfaces;

public interface ITaskService
{
    Task<TaskDto?> GetByIdAsync(string id);
    Task<IEnumerable<TaskDto>> GetAllAsync();
    Task<TaskDto> CreateAsync(CreateTaskDto dto);
    Task AssignTranslatorAsync(AssignTaskDto dto);
    Task UpdateStatusAsync(string taskId, Core.Enums.TaskStatus status);
    Task SubmitTranslationAsync(string taskId);
    Task ApproveAsync(string taskId, string reviewerId, string notes);
    Task RejectAsync(string taskId, string reviewerId, string notes);
    Task<IEnumerable<TaskDto>> GetByStatusAsync(Core.Enums.TaskStatus status);
    Task<IEnumerable<TaskDto>> GetByTranslatorIdAsync(string translatorId);
}
