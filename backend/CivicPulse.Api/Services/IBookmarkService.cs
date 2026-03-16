using CivicPulse.Api.Dtos;

namespace CivicPulse.Api.Services;

public interface IBookmarkService
{
    Task<bool> AddBookmarkAsync(string userId, int eventId);
    Task<bool> RemoveBookmarkAsync(string userId, int eventId);
    Task<List<EventDto>> GetUserBookmarksAsync(string userId);
}
