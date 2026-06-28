using TMS.Core.Enums;

namespace TMS.Core.DTOs;

public class UserDto
{
    public string Id { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; }
    public UserStatus Status { get; set; }
    public int DailyTarget { get; set; }
    public double HourlyRate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string MainService { get; set; } = string.Empty;
    public string ExperienceLevel { get; set; } = string.Empty;
}

public class CreateUserDto
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; } = DateTime.UtcNow;
    public UserStatus Status { get; set; } = UserStatus.Active;
    public int DailyTarget { get; set; } = 0;
    public double HourlyRate { get; set; } = 0;
    public string MainService { get; set; } = string.Empty;
    public string ExperienceLevel { get; set; } = string.Empty;
}

public class UpdateUserDto
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; }
    public UserStatus Status { get; set; }
    public int DailyTarget { get; set; }
    public double HourlyRate { get; set; }
    public string MainService { get; set; } = string.Empty;
    public string ExperienceLevel { get; set; } = string.Empty;
}

public class ChangeUserStatusDto
{
    public UserStatus Status { get; set; }
}
