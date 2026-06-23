using TMS.Core.Entities;

namespace TMS.Core.Interfaces;

public interface IServiceRateRepository : IRepository<ServiceRate>
{
    Task<ServiceRate?> GetByServiceAndRoleAsync(string serviceType, string role);
    Task<List<ServiceRate>> GetByServiceTypeAsync(string serviceType);
}
