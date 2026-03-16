using CivicPulse.Api.Data;
using CivicPulse.Api.Dtos;
using CivicPulse.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CivicPulse.Api.Services;

public class AdminService : IAdminService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _db;
    private readonly ISearchService _searchService;

    public AdminService(UserManager<AppUser> userManager, AppDbContext db, ISearchService searchService)
    {
        _userManager = userManager;
        _db = db;
        _searchService = searchService;
    }

    public async Task<List<AdminUserDto>> GetAllUsersAsync()
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

        return result;
    }

    public async Task<bool> ChangeRoleAsync(string userId, string role)
    {
        var validRoles = new[] { "Citizen", "Organizer", "Admin" };
        if (!validRoles.Contains(role))
            return false;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, role);

        return true;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        await _userManager.DeleteAsync(user);
        return true;
    }

    public async Task<List<EventDto>> GetAllEventsAsync()
    {
        return await _db.Events
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
    }

    public async Task<bool> ToggleApproveAsync(int eventId)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return false;

        ev.IsApproved = !ev.IsApproved;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteEventAsync(int eventId)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev == null) return false;

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        await _searchService.DeleteEventAsync(eventId.ToString());
        return true;
    }
}
