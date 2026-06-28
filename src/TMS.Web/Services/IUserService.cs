using TMS.Core.DTOs;
using TMS.Core.Enums;

namespace TMS.Web.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<bool> CreateUserAsync(CreateUserDto user);
    Task<bool> UpdateUserAsync(string id, UpdateUserDto user);
    Task<bool> DeleteUserAsync(string id);
    Task<bool> ChangeStatusAsync(string id, UserStatus status);
}
