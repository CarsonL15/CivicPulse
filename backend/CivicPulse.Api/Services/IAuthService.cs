using CivicPulse.Api.Dtos;

namespace CivicPulse.Api.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<UserDto?> GetUserAsync(string userId);
}
