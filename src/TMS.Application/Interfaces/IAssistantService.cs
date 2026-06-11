using TMS.Core.DTOs;

namespace TMS.Application.Interfaces;

public interface IAssistantService
{
    Task<TranslatorDto?> SuggestTranslatorAsync(string taskId);
    Task<string> CheckQualityAsync(string taskId);
    Task<string> ReviewTranslationAsync(string taskId);
    Task<string> GenerateReportAsync(string taskId);
}
