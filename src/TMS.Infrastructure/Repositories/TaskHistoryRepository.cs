using MongoDB.Driver;
using TMS.Core.Entities;
using TMS.Core.Interfaces;
using TMS.Infrastructure.Data;

namespace TMS.Infrastructure.Repositories;

public class TaskHistoryRepository : MongoRepository<TaskHistory>, ITaskHistoryRepository
{
    public TaskHistoryRepository(MongoDbContext context) : base(context.TaskHistories)
    {
    }

    public async Task<List<TaskHistory>> GetByTaskIdAsync(string taskId)
    {
        var filter = Builders<TaskHistory>.Filter.Eq(x => x.TaskId, taskId);
        return await _collection.Find(filter).ToListAsync();
    }
}
