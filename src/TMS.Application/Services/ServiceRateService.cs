using Microsoft.Extensions.Logging;
using TMS.Core.DTOs;
using TMS.Core.Entities;
using TMS.Core.Interfaces;

namespace TMS.Application.Services;

public class ServiceRateService : IServiceRateService
{
    private readonly IServiceRateRepository _repository;
    private readonly ILogger<ServiceRateService> _logger;

    public ServiceRateService(IServiceRateRepository repository, ILogger<ServiceRateService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<ServiceRateDto>> GetAllAsync()
    {
        var rates = await _repository.GetAllAsync();
        return rates.Select(r => new ServiceRateDto
        {
            Id = r.Id,
            ServiceType = r.ServiceType,
            Role = r.Role,
            WordsPerHour = r.WordsPerHour
        }).ToList();
    }

    public async Task<ServiceRateDto?> GetByIdAsync(string id)
    {
        var rate = await _repository.GetByIdAsync(id);
        if (rate == null) return null;

        return new ServiceRateDto
        {
            Id = rate.Id,
            ServiceType = rate.ServiceType,
            Role = rate.Role,
            WordsPerHour = rate.WordsPerHour
        };
    }

    public async Task<ServiceRateDto> CreateAsync(CreateServiceRateDto dto)
    {
        var rate = new ServiceRate
        {
            ServiceType = dto.ServiceType,
            Role = dto.Role,
            WordsPerHour = dto.WordsPerHour,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(rate);
        _logger.LogInformation($"Service rate created: {dto.ServiceType} - {dto.Role} = {dto.WordsPerHour} words/hour");

        return new ServiceRateDto
        {
            Id = created.Id,
            ServiceType = created.ServiceType,
            Role = created.Role,
            WordsPerHour = created.WordsPerHour
        };
    }

    public async Task<bool> UpdateAsync(string id, CreateServiceRateDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        existing.ServiceType = dto.ServiceType;
        existing.Role = dto.Role;
        existing.WordsPerHour = dto.WordsPerHour;
        existing.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(id, existing);
        _logger.LogInformation($"Service rate updated: {id}");
        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        await _repository.DeleteAsync(id);
        _logger.LogInformation($"Service rate deleted: {id}");
        return true;
    }

    public async Task<ServiceRateDto?> GetByServiceAndRoleAsync(string serviceType, string role)
    {
        var rate = await _repository.GetByServiceAndRoleAsync(serviceType, role);
        if (rate == null) return null;

        return new ServiceRateDto
        {
            Id = rate.Id,
            ServiceType = rate.ServiceType,
            Role = rate.Role,
            WordsPerHour = rate.WordsPerHour
        };
    }
}
