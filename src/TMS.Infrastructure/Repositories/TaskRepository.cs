using MongoDB.Driver;
using TMS.Core.Entities;
using TMS.Core.Interfaces;
using TMS.Infrastructure.Data;

namespace TMS.Infrastructure.Repositories;

public class TaskRepository : MongoRepository<TranslationTask>, ITaskRepository
{
    public TaskRepository(MongoDbContext context) : base(context.Tasks)
    {
    }

    public async Task<IEnumerable<TranslationTask>> GetByStatusAsync(Core.Enums.TaskStatus status)
    {
        return await _collection.Find(t => t.Status == status).ToListAsync();
    }

    public async Task<IEnumerable<TranslationTask>> GetByTranslatorIdAsync(string translatorId)
    {
        return await _collection.Find(t => t.AssignedTranslatorId == translatorId).ToListAsync();
    }

    public async Task<IEnumerable<TranslationTask>> GetByCreatorIdAsync(string creatorId)
    {
        return await _collection.Find(t => t.CreatedBy == creatorId).ToListAsync();
    }
}
