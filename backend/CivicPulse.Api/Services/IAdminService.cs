using CivicPulse.Api.Dtos;

namespace CivicPulse.Api.Services;

public interface IAdminService
{
    Task<List<AdminUserDto>> GetAllUsersAsync();
    Task<bool> ChangeRoleAsync(string userId, string role);
    Task<bool> DeleteUserAsync(string userId);
    Task<List<EventDto>> GetAllEventsAsync();
    Task<bool> ToggleApproveAsync(int eventId);
    Task<bool> DeleteEventAsync(int eventId);
}
