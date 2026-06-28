using TMS.Core.DTOs;

namespace TMS.Application.Interfaces;

public interface IQualityScoreService
{
    Task<QualityScoreDto?> GetByIdAsync(string id);
    Task<IEnumerable<QualityScoreDto>> GetAllAsync();
    Task<IEnumerable<QualityScoreDto>> GetByTranslatorIdAsync(string translatorId);
    Task<IEnumerable<QualityScoreDto>> GetByReviewerIdAsync(string reviewerId);
    Task<IEnumerable<QualityScoreDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<QualityScoreDto> CreateAsync(CreateQualityScoreDto dto);
    Task UpdateAsync(string id, UpdateQualityScoreDto dto);
    Task DeleteAsync(string id);
}
