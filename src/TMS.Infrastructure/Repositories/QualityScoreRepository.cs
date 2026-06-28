using MongoDB.Driver;
using TMS.Core.Entities;
using TMS.Core.Interfaces;
using TMS.Infrastructure.Data;

namespace TMS.Infrastructure.Repositories;

public class QualityScoreRepository : MongoRepository<QualityScoreRecord>, IQualityScoreRepository
{
    public QualityScoreRepository(MongoDbContext context) : base(context.QualityScores)
    {
    }

    public async Task<IEnumerable<QualityScoreRecord>> GetByTranslatorIdAsync(string translatorId)
    {
        return await _collection.Find(q => q.TranslatorId == translatorId).ToListAsync();
    }

    public async Task<IEnumerable<QualityScoreRecord>> GetByReviewerIdAsync(string reviewerId)
    {
        return await _collection.Find(q => q.ReviewerId == reviewerId).ToListAsync();
    }

    public async Task<IEnumerable<QualityScoreRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _collection.Find(q => q.Date >= startDate && q.Date <= endDate).ToListAsync();
    }
}
