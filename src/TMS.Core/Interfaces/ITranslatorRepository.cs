using TMS.Core.Entities;

namespace TMS.Core.Interfaces;

public interface ITranslatorRepository : IRepository<Translator>
{
    Task<Translator?> GetByUserIdAsync(string userId);
    Task<IEnumerable<Translator>> GetByLanguageAsync(string language);
    Task<IEnumerable<Translator>> GetAvailableTranslatorsAsync();
}
