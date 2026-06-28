using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Core.DTOs;
using TMS.Core.Enums;

namespace TMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;

    public AuthService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        await Task.Delay(100);

        // Validate input
        if (string.IsNullOrWhiteSpace(email))
        {
            return new AuthResult
            {
                Success = false,
                Message = "Email is required"
            };
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return new AuthResult
            {
                Success = false,
                Message = "Password is required"
            };
        }

        // Basic email format validation
        if (!email.Contains("@") || !email.Contains("."))
        {
            return new AuthResult
            {
                Success = false,
                Message = "Invalid email format"
            };
        }

        // Password length validation
        if (password.Length < 2)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Password must be at least 6 characters"
            };
        }

        // Look up user from database
        var user = await _userService.GetByEmailAsync(email);
        
        if (user == null)
        {
            // For demo: create default admin user if not exists
            if (email == "admin@translationgate.com")
            {
                var createUserDto = new CreateUserDto
                {
                    FullName = "Admin User",
                    Email = email,
                    Password = password,
                    Role = UserRole.Admin,
                    Status = UserStatus.Active
                };
                user = await _userService.CreateAsync(createUserDto);
            }
            else
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "User not found"
                };
            }
        }

        if (user.Status == UserStatus.Inactive)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Account is inactive. Please contact administrator."
            };
        }

        // For demo, accept any password (in production, verify password hash)
        // Generate simple token (for production, use JWT)
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return new AuthResult
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            User = new UserInfo
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.FullName,
                Role = user.Role.ToString() // Convert enum to string without spaces
            }
        };
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        await Task.Delay(10);
        
        // For demo, accept any non-empty token
        // For production, validate JWT token
        return !string.IsNullOrWhiteSpace(token);
    }
}
