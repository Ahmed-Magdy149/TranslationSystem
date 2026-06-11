using Microsoft.AspNetCore.Mvc;
using TMS.Application.Interfaces;
using TMS.Core.DTOs;
using TMS.Core.Entities;

namespace TMS.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { Success = false, Message = "Email and password are required" });
        }

        var users = await _userService.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Email == request.Email);

        if (user == null)
        {
            return Unauthorized(new { Success = false, Message = "Invalid email or password" });
        }

        // Verify password (in production, use proper password hashing)
        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { Success = false, Message = "Invalid email or password" });
        }

        if (!user.IsActive)
        {
            return Unauthorized(new { Success = false, Message = "Account is inactive" });
        }

        return Ok(new LoginResponse
        {
            Success = true,
            Message = "Login successful",
            User = new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Role,
                user.IsActive
            }
        });
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        // In production, use BCrypt or similar
        // For now, simple comparison (hashing should be implemented in UserService)
        return password == passwordHash;
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? User { get; set; }
}
