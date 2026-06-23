using TMS.Core.DTOs;

namespace TMS.Core.Interfaces;

public interface IServiceRateService
{
    Task<List<ServiceRateDto>> GetAllAsync();
    Task<ServiceRateDto?> GetByIdAsync(string id);
    Task<ServiceRateDto> CreateAsync(CreateServiceRateDto dto);
    Task<bool> UpdateAsync(string id, CreateServiceRateDto dto);
    Task<bool> DeleteAsync(string id);
    Task<ServiceRateDto?> GetByServiceAndRoleAsync(string serviceType, string role);
}
