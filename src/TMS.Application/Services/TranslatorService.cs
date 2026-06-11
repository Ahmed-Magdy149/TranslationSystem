using Microsoft.Extensions.Logging;
using TMS.Application.Interfaces;
using TMS.Core.DTOs;
using TMS.Core.Entities;
using TMS.Core.Interfaces;

namespace TMS.Application.Services;

public class TranslatorService : ITranslatorService
{
    private readonly ITranslatorRepository _translatorRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TranslatorService> _logger;

    public TranslatorService(
        ITranslatorRepository translatorRepository,
        ITaskRepository taskRepository,
        ILogger<TranslatorService> logger)
    {
        _translatorRepository = translatorRepository;
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<TranslatorDto?> GetByIdAsync(string id)
    {
        var translator = await _translatorRepository.GetByIdAsync(id);
        return translator == null ? null : MapToDto(translator);
    }

    public async Task<IEnumerable<TranslatorDto>> GetAllAsync()
    {
        var translators = await _translatorRepository.GetAllAsync();
        return translators.Select(MapToDto);
    }

    public async Task<TranslatorDto> CreateAsync(CreateTranslatorDto dto)
    {
        var existing = await _translatorRepository.GetByUserIdAsync(dto.UserId);
        if (existing != null)
            throw new InvalidOperationException($"Translator profile already exists for user '{dto.UserId}'.");

        var translator = new Translator
        {
            UserId = dto.UserId,
            Languages = dto.Languages,
            Specialization = dto.Specialization,
            RatePerWord = dto.RatePerWord,
            AverageSpeed = dto.AverageSpeed,
            Rating = 0
        };

        await _translatorRepository.CreateAsync(translator);
        _logger.LogInformation("Translator profile created for user {UserId}", dto.UserId);
        return MapToDto(translator);
    }

    public async Task UpdateAsync(string id, UpdateTranslatorDto dto)
    {
        var translator = await _translatorRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Translator with id '{id}' not found.");

        translator.Languages = dto.Languages;
        translator.Specialization = dto.Specialization;
        translator.RatePerWord = dto.RatePerWord;
        translator.AverageSpeed = dto.AverageSpeed;

        await _translatorRepository.UpdateAsync(id, translator);
        _logger.LogInformation("Translator updated: {Id}", id);
    }

    public async Task<IEnumerable<TranslatorDto>> GetAvailableAsync()
    {
        var translators = await _translatorRepository.GetAvailableTranslatorsAsync();
        return translators.Select(MapToDto);
    }

    public async Task<IEnumerable<TranslatorDto>> GetByLanguageAsync(string language)
    {
        var translators = await _translatorRepository.GetByLanguageAsync(language);
        return translators.Select(MapToDto);
    }

    public async Task<int> CalculateWorkloadAsync(string translatorId)
    {
        var tasks = await _taskRepository.GetByTranslatorIdAsync(translatorId);
        var activeTaskCount = tasks.Count(t =>
            t.Status != Core.Enums.TaskStatus.Approved &&
            t.Status != Core.Enums.TaskStatus.Rejected);

        _logger.LogInformation("Translator {TranslatorId} workload: {Count} active tasks", translatorId, activeTaskCount);
        return activeTaskCount;
    }

    private static TranslatorDto MapToDto(Translator translator)
    {
        return new TranslatorDto
        {
            Id = translator.Id,
            UserId = translator.UserId,
            Languages = translator.Languages,
            Specialization = translator.Specialization,
            RatePerWord = translator.RatePerWord,
            AverageSpeed = translator.AverageSpeed,
            Rating = translator.Rating
        };
    }
}
