using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CivicPulse.Api.Configuration;
using CivicPulse.Api.Dtos;
using CivicPulse.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CivicPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly JwtConfig _jwtConfig;

    public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        IOptions<JwtConfig> jwtConfig)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtConfig = jwtConfig.Value;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var validRoles = new[] { "Citizen", "Organizer" };
        if (!validRoles.Contains(dto.Role))
            return BadRequest("Role must be Citizen or Organizer");

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            DisplayName = dto.DisplayName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, dto.Role);

        var token = GenerateJwtToken(user, dto.Role);
        return Ok(new AuthResponseDto
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Role = dto.Role
            }
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Unauthorized("Invalid credentials");

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            return Unauthorized("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Citizen";

        var token = GenerateJwtToken(user, role);
        return Ok(new AuthResponseDto
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Role = role
            }
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            Role = roles.FirstOrDefault() ?? "Citizen"
        });
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
