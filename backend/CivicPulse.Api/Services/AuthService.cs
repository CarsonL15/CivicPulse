using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CivicPulse.Api.Configuration;
using CivicPulse.Api.Dtos;
using CivicPulse.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CivicPulse.Api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly JwtConfig _jwtConfig;

    public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        IOptions<JwtConfig> jwtConfig)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtConfig = jwtConfig.Value;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        var validRoles = new[] { "Citizen", "Organizer" };
        if (!validRoles.Contains(dto.Role))
            return null;

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            DisplayName = dto.DisplayName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return null;

        await _userManager.AddToRoleAsync(user, dto.Role);

        var token = GenerateJwtToken(user, dto.Role);
        return new AuthResponseDto
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Role = dto.Role
            }
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Citizen";

        var token = GenerateJwtToken(user, role);
        return new AuthResponseDto
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Role = role
            }
        };
    }

    public async Task<UserDto?> GetUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            Role = roles.FirstOrDefault() ?? "Citizen"
        };
    }

    private string GenerateJwtToken(AppUser user, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
