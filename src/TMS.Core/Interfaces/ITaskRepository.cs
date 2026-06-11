using TMS.Core.Entities;

namespace TMS.Core.Interfaces;

public interface ITaskRepository : IRepository<TranslationTask>
{
    Task<IEnumerable<TranslationTask>> GetByStatusAsync(Enums.TaskStatus status);
    Task<IEnumerable<TranslationTask>> GetByTranslatorIdAsync(string translatorId);
    Task<IEnumerable<TranslationTask>> GetByCreatorIdAsync(string creatorId);
}
