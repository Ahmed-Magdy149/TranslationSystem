using TMS.Core.DTOs;
using TMS.Core.Enums;

namespace TMS.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(string id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task UpdateAsync(string id, UpdateUserDto dto);
    Task ChangeStatusAsync(string id, UserStatus status);
    Task ChangeRoleAsync(string id, UserRole role);
    Task<UserDto?> GetByEmailAsync(string email);
}
