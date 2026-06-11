using TMS.Application.DTOs;

namespace TMS.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<bool> ValidateTokenAsync(string token);
}
