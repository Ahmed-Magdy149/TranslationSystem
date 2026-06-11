using MongoDB.Driver;
using TMS.Core.Entities;
using TMS.Core.Interfaces;
using TMS.Infrastructure.Data;

namespace TMS.Infrastructure.Repositories;

public class TranslatorRepository : MongoRepository<Translator>, ITranslatorRepository
{
    private readonly MongoDbContext _context;

    public TranslatorRepository(MongoDbContext context) : base(context.Translators)
    {
        _context = context;
    }

    public async Task<Translator?> GetByUserIdAsync(string userId)
    {
        return await _collection.Find(t => t.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Translator>> GetByLanguageAsync(string language)
    {
        return await _collection.Find(t => t.Languages.Contains(language)).ToListAsync();
    }

    public async Task<IEnumerable<Translator>> GetAvailableTranslatorsAsync()
    {
        var allTranslators = await _collection.Find(_ => true).ToListAsync();
        var availableTranslators = new List<Translator>();

        foreach (var translator in allTranslators)
        {
            var activeTasks = await _context.Tasks
                .Find(t => t.AssignedTranslatorId == translator.Id &&
                           t.Status != Core.Enums.TaskStatus.Approved &&
                           t.Status != Core.Enums.TaskStatus.Rejected)
                .CountDocumentsAsync();

            if (activeTasks < 3)
                availableTranslators.Add(translator);
        }

        return availableTranslators;
    }
}
