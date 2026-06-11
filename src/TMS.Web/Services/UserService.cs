using System.Net.Http.Json;
using TMS.Core.DTOs;

namespace TMS.Web.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "/api/users";

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<UserDto>>(BaseUrl);
            return response ?? new List<UserDto>();
        }
        catch
        {
            return new List<UserDto>();
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserDto>($"{BaseUrl}/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> CreateUserAsync(CreateUserDto user)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, user);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateUserAsync(string id, UpdateUserDto user)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", user);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(string id)
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
