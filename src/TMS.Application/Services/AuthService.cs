using TMS.Application.DTOs;
using TMS.Application.Interfaces;

namespace TMS.Application.Services;

public class AuthService : IAuthService
{
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

        // Demo authentication - accept any valid format
        // For production, replace with database user lookup and password hash comparison
        var user = new UserInfo
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            Name = "Sarah Reed",
            Role = "Project Manager"
        };

        // Generate simple token (for production, use JWT)
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return new AuthResult
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            User = user
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
