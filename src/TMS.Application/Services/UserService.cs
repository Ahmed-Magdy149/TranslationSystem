using Microsoft.Extensions.Logging;
using TMS.Application.Interfaces;
using TMS.Core.DTOs;
using TMS.Core.Entities;
using TMS.Core.Enums;
using TMS.Core.Interfaces;

namespace TMS.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var existing = await _userRepository.GetByEmailAsync(dto.Email);
        if (existing != null)
            throw new InvalidOperationException($"User with email '{dto.Email}' already exists.");

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            PasswordHash = HashPassword(dto.Password),
            Role = dto.Role,
            Language = dto.Language,
            Specialization = dto.Specialization,
            JoiningDate = dto.JoiningDate,
            Status = dto.Status,
            DailyTarget = dto.DailyTarget,
            HourlyRate = dto.HourlyRate,
            MainService = dto.MainService,
            ExperienceLevel = dto.ExperienceLevel,
            CreatedDate = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);
        _logger.LogInformation("User created: {Email} with role {Role}", user.Email, user.Role);
        return MapToDto(user);
    }

    public async Task UpdateAsync(string id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"User with id '{id}' not found.");

        var existingByEmail = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingByEmail != null && existingByEmail.Id != id)
            throw new InvalidOperationException($"Email '{dto.Email}' already in use by another user.");

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.PhoneNumber = dto.PhoneNumber;
        user.Role = dto.Role;
        user.Language = dto.Language;
        user.Specialization = dto.Specialization;
        user.JoiningDate = dto.JoiningDate;
        user.Status = dto.Status;
        user.DailyTarget = dto.DailyTarget;
        user.HourlyRate = dto.HourlyRate;
        user.MainService = dto.MainService;
        user.ExperienceLevel = dto.ExperienceLevel;

        await _userRepository.UpdateAsync(id, user);
        _logger.LogInformation("User updated: {Id}", id);
    }

    public async Task ChangeStatusAsync(string id, UserStatus status)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"User with id '{id}' not found.");

        user.Status = status;
        await _userRepository.UpdateAsync(id, user);
        _logger.LogInformation("User status changed: {Id} -> {Status}", id, status);
    }

    public async Task ChangeRoleAsync(string id, UserRole role)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"User with id '{id}' not found.");

        user.Role = role;
        await _userRepository.UpdateAsync(id, user);
        _logger.LogInformation("User role changed: {Id} -> {Role}", id, role);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user == null ? null : MapToDto(user);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            Language = user.Language,
            Specialization = user.Specialization,
            JoiningDate = user.JoiningDate,
            Status = user.Status,
            DailyTarget = user.DailyTarget,
            HourlyRate = user.HourlyRate,
            CreatedDate = user.CreatedDate,
            LastLoginDate = user.LastLoginDate,
            MainService = user.MainService,
            ExperienceLevel = user.ExperienceLevel
        };
    }

    private static string HashPassword(string password)
    {
        return BCryptStyle(password);
    }

    private static string BCryptStyle(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
