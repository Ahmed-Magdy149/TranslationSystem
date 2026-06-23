using MongoDB.Driver;
using TMS.Core.Entities;
using TMS.Core.Interfaces;
using TMS.Infrastructure.Data;

namespace TMS.Infrastructure.Repositories;

public class ServiceRateRepository : MongoRepository<ServiceRate>, IServiceRateRepository
{
    public ServiceRateRepository(MongoDbContext context) : base(context.ServiceRates)
    {
    }

    public async Task<ServiceRate?> GetByServiceAndRoleAsync(string serviceType, string role)
    {
        var filter = Builders<ServiceRate>.Filter.And(
            Builders<ServiceRate>.Filter.Eq(x => x.ServiceType, serviceType),
            Builders<ServiceRate>.Filter.Eq(x => x.Role, role)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<ServiceRate>> GetByServiceTypeAsync(string serviceType)
    {
        var filter = Builders<ServiceRate>.Filter.Eq(x => x.ServiceType, serviceType);
        return await _collection.Find(filter).ToListAsync();
    }
}
