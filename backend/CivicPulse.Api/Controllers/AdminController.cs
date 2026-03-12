using CivicPulse.Api.Data;
using CivicPulse.Api.Dtos;
using CivicPulse.Api.Models;
using CivicPulse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CivicPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _db;
    private readonly ISearchService _searchService;

    public AdminController(UserManager<AppUser> userManager, AppDbContext db, ISearchService searchService)
    {
        _userManager = userManager;
        _db = db;
        _searchService = searchService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<AdminUserDto>>> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = new List<AdminUserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new AdminUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Role = roles.FirstOrDefault() ?? "Citizen",
                CreatedAt = user.CreatedAt
            });
        }

        return Ok(result);
    }

    [HttpPut("users/{id}/role")]
    public async Task<ActionResult> ChangeRole(string id, ChangeRoleDto dto)
    {
        var validRoles = new[] { "Citizen", "Organizer", "Admin" };
        if (!validRoles.Contains(dto.Role))
            return BadRequest("Invalid role");

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, dto.Role);

        return Ok();
    }

    [HttpDelete("users/{id}")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        await _userManager.DeleteAsync(user);
        return NoContent();
    }

    [HttpGet("events")]
    public async Task<ActionResult<List<EventDto>>> GetAllEvents()
    {
        var events = await _db.Events
            .Include(e => e.Organizer)
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                EventDate = e.EventDate,
                Time = e.Time,
                Location = e.Location,
                Category = e.Category.ToString(),
                AiSummary = e.AiSummary,
                WhyItMatters = e.WhyItMatters,
                OrganizerName = e.Organizer.DisplayName,
                OrganizerId = e.OrganizerId,
                IsApproved = e.IsApproved
            })
            .ToListAsync();

        return Ok(events);
    }

    [HttpPut("events/{id}/approve")]
    public async Task<ActionResult> ToggleApprove(int id)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev == null) return NotFound();

        ev.IsApproved = !ev.IsApproved;
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("events/{id}")]
    public async Task<ActionResult> DeleteEvent(int id)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev == null) return NotFound();

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        await _searchService.DeleteEventAsync(id.ToString());
        return NoContent();
    }
}
