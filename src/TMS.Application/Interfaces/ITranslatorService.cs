using TMS.Core.DTOs;

namespace TMS.Application.Interfaces;

public interface ITranslatorService
{
    Task<TranslatorDto?> GetByIdAsync(string id);
    Task<IEnumerable<TranslatorDto>> GetAllAsync();
    Task<TranslatorDto> CreateAsync(CreateTranslatorDto dto);
    Task UpdateAsync(string id, UpdateTranslatorDto dto);
    Task<IEnumerable<TranslatorDto>> GetAvailableAsync();
    Task<IEnumerable<TranslatorDto>> GetByLanguageAsync(string language);
    Task<int> CalculateWorkloadAsync(string translatorId);
}
