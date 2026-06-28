using TMS.Core.DTOs;

namespace TMS.Web.Services;

public interface IQualityScoreService
{
    Task<List<QualityScoreDto>> GetAllAsync();
    Task<QualityScoreDto?> GetByIdAsync(string id);
    Task<List<QualityScoreDto>> GetByTranslatorAsync(string translatorId);
    Task<List<QualityScoreDto>> GetByReviewerAsync(string reviewerId);
    Task<bool> CreateAsync(CreateQualityScoreDto dto);
    Task<bool> UpdateAsync(string id, UpdateQualityScoreDto dto);
    Task<bool> DeleteAsync(string id);
}
