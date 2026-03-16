using CivicPulse.Api.Dtos;
using CivicPulse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CivicPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<AdminUserDto>>> GetUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpPut("users/{id}/role")]
    public async Task<ActionResult> ChangeRole(string id, ChangeRoleDto dto)
    {
        var result = await _adminService.ChangeRoleAsync(id, dto.Role);
        if (!result) return NotFound();
        return Ok();
    }

    [HttpDelete("users/{id}")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        var result = await _adminService.DeleteUserAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpGet("events")]
    public async Task<ActionResult<List<EventDto>>> GetAllEvents()
    {
        var events = await _adminService.GetAllEventsAsync();
        return Ok(events);
    }

    [HttpPut("events/{id}/approve")]
    public async Task<ActionResult> ToggleApprove(int id)
    {
        var result = await _adminService.ToggleApproveAsync(id);
        if (!result) return NotFound();
        return Ok();
    }

    [HttpDelete("events/{id}")]
    public async Task<ActionResult> DeleteEvent(int id)
    {
        var result = await _adminService.DeleteEventAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
