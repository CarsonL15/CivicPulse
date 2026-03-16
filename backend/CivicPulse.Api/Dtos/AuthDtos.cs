using System.ComponentModel.DataAnnotations;

namespace CivicPulse.Api.Dtos;

public class RegisterDto
{
    [Required] public string Email { get; set; } = string.Empty;
    [Required] [MinLength(6)] public string Password { get; set; } = string.Empty;
    [Required] public string DisplayName { get; set; } = string.Empty;
    [Required] public string Role { get; set; } = "Citizen"; // Citizen or Organizer
}

public class LoginDto
{
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
