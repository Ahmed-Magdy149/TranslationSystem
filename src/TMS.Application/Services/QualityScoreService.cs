using Microsoft.Extensions.Logging;
using TMS.Application.Interfaces;
using TMS.Core.DTOs;
using TMS.Core.Entities;
using TMS.Core.Interfaces;

namespace TMS.Application.Services;

public class QualityScoreService : IQualityScoreService
{
    private readonly IQualityScoreRepository _qualityScoreRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<QualityScoreService> _logger;

    public QualityScoreService(IQualityScoreRepository qualityScoreRepository, IUserRepository userRepository, ILogger<QualityScoreService> logger)
    {
        _qualityScoreRepository = qualityScoreRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<QualityScoreDto?> GetByIdAsync(string id)
    {
        var score = await _qualityScoreRepository.GetByIdAsync(id);
        return score == null ? null : MapToDto(score);
    }

    public async Task<IEnumerable<QualityScoreDto>> GetAllAsync()
    {
        var scores = await _qualityScoreRepository.GetAllAsync();
        return scores.Select(MapToDto);
    }

    public async Task<IEnumerable<QualityScoreDto>> GetByTranslatorIdAsync(string translatorId)
    {
        var scores = await _qualityScoreRepository.GetByTranslatorIdAsync(translatorId);
        return scores.Select(MapToDto);
    }

    public async Task<IEnumerable<QualityScoreDto>> GetByReviewerIdAsync(string reviewerId)
    {
        var scores = await _qualityScoreRepository.GetByReviewerIdAsync(reviewerId);
        return scores.Select(MapToDto);
    }

    public async Task<IEnumerable<QualityScoreDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var scores = await _qualityScoreRepository.GetByDateRangeAsync(startDate, endDate);
        return scores.Select(MapToDto);
    }

    public async Task<QualityScoreDto> CreateAsync(CreateQualityScoreDto dto)
    {
        var translator = await _userRepository.GetByIdAsync(dto.TranslatorId)
            ?? throw new KeyNotFoundException($"Translator with id '{dto.TranslatorId}' not found.");

        var reviewer = await _userRepository.GetByIdAsync(dto.ReviewerId)
            ?? throw new KeyNotFoundException($"Reviewer with id '{dto.ReviewerId}' not found.");

        var score = new QualityScoreRecord
        {
            Date = dto.Date,
            TranslatorId = dto.TranslatorId,
            TranslatorName = translator.FullName,
            ReviewerId = dto.ReviewerId,
            ReviewerName = reviewer.FullName,
            ProjectName = dto.ProjectName,
            ServiceType = dto.ServiceType,
            ReviewedWords = dto.ReviewedWords,
            ReviewHours = dto.ReviewHours,
            MaximumScore = dto.MaximumScore,
            QualityScore = dto.QualityScore,
            MajorErrors = dto.MajorErrors,
            MinorErrors = dto.MinorErrors,
            Comments = dto.Comments
        };

        CalculateMetrics(score);

        await _qualityScoreRepository.CreateAsync(score);
        _logger.LogInformation("Quality score created for translator {Translator} by reviewer {Reviewer}", translator.FullName, reviewer.FullName);
        return MapToDto(score);
    }

    public async Task UpdateAsync(string id, UpdateQualityScoreDto dto)
    {
        var score = await _qualityScoreRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Quality score with id '{id}' not found.");

        var translator = await _userRepository.GetByIdAsync(dto.TranslatorId)
            ?? throw new KeyNotFoundException($"Translator with id '{dto.TranslatorId}' not found.");

        score.Date = dto.Date;
        score.TranslatorId = dto.TranslatorId;
        score.TranslatorName = translator.FullName;
        score.ProjectName = dto.ProjectName;
        score.ServiceType = dto.ServiceType;
        score.ReviewedWords = dto.ReviewedWords;
        score.ReviewHours = dto.ReviewHours;
        score.MaximumScore = dto.MaximumScore;
        score.QualityScore = dto.QualityScore;
        score.MajorErrors = dto.MajorErrors;
        score.MinorErrors = dto.MinorErrors;
        score.Comments = dto.Comments;
        score.UpdatedAt = DateTime.UtcNow;

        CalculateMetrics(score);

        await _qualityScoreRepository.UpdateAsync(id, score);
        _logger.LogInformation("Quality score updated: {Id}", id);
    }

    public async Task DeleteAsync(string id)
    {
        var score = await _qualityScoreRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Quality score with id '{id}' not found.");

        await _qualityScoreRepository.DeleteAsync(id);
        _logger.LogInformation("Quality score deleted: {Id}", id);
    }

    private static void CalculateMetrics(QualityScoreRecord score)
    {
        score.QualityPercentage = score.MaximumScore > 0
            ? Math.Round((score.QualityScore / score.MaximumScore) * 100, 2)
            : 0;

        score.PerformanceStatus = score.QualityPercentage switch
        {
            >= 95 and <= 100 => "Excellent",
            >= 80 and < 95 => "Good",
            _ => "Needs Improvement"
        };
    }

    private static QualityScoreDto MapToDto(QualityScoreRecord score)
    {
        return new QualityScoreDto
        {
            Id = score.Id,
            Date = score.Date,
            TranslatorId = score.TranslatorId,
            TranslatorName = score.TranslatorName,
            ReviewerId = score.ReviewerId,
            ReviewerName = score.ReviewerName,
            ProjectName = score.ProjectName,
            ServiceType = score.ServiceType,
            ReviewedWords = score.ReviewedWords,
            ReviewHours = score.ReviewHours,
            MaximumScore = score.MaximumScore,
            QualityScore = score.QualityScore,
            QualityPercentage = score.QualityPercentage,
            MajorErrors = score.MajorErrors,
            MinorErrors = score.MinorErrors,
            ErrorCount = score.ErrorCount,
            Comments = score.Comments,
            PerformanceStatus = score.PerformanceStatus,
            CreatedAt = score.CreatedAt
        };
    }
}
