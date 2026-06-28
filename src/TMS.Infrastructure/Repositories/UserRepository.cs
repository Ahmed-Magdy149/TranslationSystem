using MongoDB.Driver;
using TMS.Core.Entities;
using TMS.Core.Enums;
using TMS.Core.Interfaces;
using TMS.Infrastructure.Data;

namespace TMS.Infrastructure.Repositories;

public class UserRepository : MongoRepository<User>, IUserRepository
{
    public UserRepository(MongoDbContext context) : base(context.Users)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _collection.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _collection.Find(u => u.Status == UserStatus.Active).ToListAsync();
    }
}
