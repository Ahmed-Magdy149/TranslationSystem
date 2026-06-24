using TMS.Core.Entities;

namespace TMS.Core.Interfaces;

public interface ITaskHistoryRepository : IRepository<TaskHistory>
{
    Task<List<TaskHistory>> GetByTaskIdAsync(string taskId);
}
