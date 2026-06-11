using TMS.Core.DTOs;

namespace TMS.Web.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<bool> CreateUserAsync(CreateUserDto user);
    Task<bool> UpdateUserAsync(string id, UpdateUserDto user);
    Task<bool> DeleteUserAsync(string id);
}
