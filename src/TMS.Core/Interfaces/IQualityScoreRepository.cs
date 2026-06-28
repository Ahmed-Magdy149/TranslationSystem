using TMS.Core.Entities;

namespace TMS.Core.Interfaces;

public interface IQualityScoreRepository : IRepository<QualityScoreRecord>
{
    Task<IEnumerable<QualityScoreRecord>> GetByTranslatorIdAsync(string translatorId);
    Task<IEnumerable<QualityScoreRecord>> GetByReviewerIdAsync(string reviewerId);
    Task<IEnumerable<QualityScoreRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}
