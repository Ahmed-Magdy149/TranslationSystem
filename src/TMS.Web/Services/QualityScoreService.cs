using System.Net.Http.Json;
using TMS.Core.DTOs;

namespace TMS.Web.Services;

public class QualityScoreService : IQualityScoreService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "/api/qualityscores";

    public QualityScoreService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<QualityScoreDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<QualityScoreDto>>(BaseUrl);
            return response ?? new List<QualityScoreDto>();
        }
        catch
        {
            return new List<QualityScoreDto>();
        }
    }

    public async Task<QualityScoreDto?> GetByIdAsync(string id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<QualityScoreDto>($"{BaseUrl}/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<QualityScoreDto>> GetByTranslatorAsync(string translatorId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<QualityScoreDto>>($"{BaseUrl}/translator/{translatorId}");
            return response ?? new List<QualityScoreDto>();
        }
        catch
        {
            return new List<QualityScoreDto>();
        }
    }

    public async Task<List<QualityScoreDto>> GetByReviewerAsync(string reviewerId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<QualityScoreDto>>($"{BaseUrl}/reviewer/{reviewerId}");
            return response ?? new List<QualityScoreDto>();
        }
        catch
        {
            return new List<QualityScoreDto>();
        }
    }

    public async Task<bool> CreateAsync(CreateQualityScoreDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(string id, UpdateQualityScoreDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", dto);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
