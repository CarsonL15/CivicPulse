using System.Security.Claims;
using CivicPulse.Api.Dtos;
using CivicPulse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CivicPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (result == null) return BadRequest("Registration failed");
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result == null) return Unauthorized("Invalid credentials");
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _authService.GetUserAsync(userId!);
        if (user == null) return NotFound();
        return Ok(user);
    }
}
