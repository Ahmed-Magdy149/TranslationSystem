using Microsoft.Extensions.Logging;
using TMS.Application.Interfaces;
using TMS.Core.DTOs;

namespace TMS.Application.Services;

public class AssistantService : IAssistantService
{
    private readonly ILogger<AssistantService> _logger;

    public AssistantService(ILogger<AssistantService> logger)
    {
        _logger = logger;
    }

    public Task<TranslatorDto?> SuggestTranslatorAsync(string taskId)
    {
        _logger.LogWarning("AI SuggestTranslator not implemented yet. TaskId: {TaskId}", taskId);
        throw new NotImplementedException("AI Translator Suggestion feature is coming soon.");
    }

    public Task<string> CheckQualityAsync(string taskId)
    {
        _logger.LogWarning("AI CheckQuality not implemented yet. TaskId: {TaskId}", taskId);
        throw new NotImplementedException("AI Quality Checking feature is coming soon.");
    }

    public Task<string> ReviewTranslationAsync(string taskId)
    {
        _logger.LogWarning("AI ReviewTranslation not implemented yet. TaskId: {TaskId}", taskId);
        throw new NotImplementedException("AI Translation Review feature is coming soon.");
    }

    public Task<string> GenerateReportAsync(string taskId)
    {
        _logger.LogWarning("AI GenerateReport not implemented yet. TaskId: {TaskId}", taskId);
        throw new NotImplementedException("AI Smart Reports feature is coming soon.");
    }
}
