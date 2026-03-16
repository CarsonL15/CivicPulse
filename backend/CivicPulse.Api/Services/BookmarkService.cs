using CivicPulse.Api.Data;
using CivicPulse.Api.Dtos;
using CivicPulse.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CivicPulse.Api.Services;

public class BookmarkService : IBookmarkService
{
    private readonly AppDbContext _db;

    public BookmarkService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> AddBookmarkAsync(string userId, int eventId)
    {
        var exists = await _db.Bookmarks.AnyAsync(b => b.UserId == userId && b.EventId == eventId);
        if (exists) return false;

        var eventExists = await _db.Events.AnyAsync(e => e.Id == eventId);
        if (!eventExists) return false;

        _db.Bookmarks.Add(new Bookmark { UserId = userId, EventId = eventId });
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveBookmarkAsync(string userId, int eventId)
    {
        var bookmark = await _db.Bookmarks
            .FirstOrDefaultAsync(b => b.UserId == userId && b.EventId == eventId);
        if (bookmark == null) return false;

        _db.Bookmarks.Remove(bookmark);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<EventDto>> GetUserBookmarksAsync(string userId)
    {
        var bookmarks = await _db.Bookmarks
            .Include(b => b.Event)
            .ThenInclude(e => e.Organizer)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return bookmarks.Select(b => new EventDto
        {
            Id = b.Event.Id,
            Title = b.Event.Title,
            Description = b.Event.Description,
            EventDate = b.Event.EventDate,
            Time = b.Event.Time,
            Location = b.Event.Location,
            Category = b.Event.Category.ToString(),
            AiSummary = b.Event.AiSummary,
            WhyItMatters = b.Event.WhyItMatters,
            OrganizerName = b.Event.Organizer?.DisplayName ?? "",
            OrganizerId = b.Event.OrganizerId,
            IsBookmarked = true,
            IsApproved = b.Event.IsApproved
        }).ToList();
    }
}
