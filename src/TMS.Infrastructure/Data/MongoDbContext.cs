using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TMS.Core.Entities;

namespace TMS.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<Translator> Translators => _database.GetCollection<Translator>("Translators");
    public IMongoCollection<TranslationTask> Tasks => _database.GetCollection<TranslationTask>("Tasks");
    public IMongoCollection<TaskHistory> TaskHistories => _database.GetCollection<TaskHistory>("TaskHistories");
    public IMongoCollection<Review> Reviews => _database.GetCollection<Review>("Reviews");
    public IMongoCollection<ServiceRate> ServiceRates => _database.GetCollection<ServiceRate>("ServiceRates");
    
    public IMongoDatabase Database => _database;
}
