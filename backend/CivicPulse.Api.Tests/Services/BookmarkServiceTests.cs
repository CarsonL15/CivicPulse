using CivicPulse.Api.Data;
using CivicPulse.Api.Models;
using CivicPulse.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace CivicPulse.Api.Tests.Services;

public class BookmarkServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static async Task<(AppDbContext db, string userId, int eventId)> SeedDataAsync()
    {
        var db = CreateContext();
        var user = new AppUser { Id = "user1", UserName = "test@test.com", Email = "test@test.com", DisplayName = "Test" };
        var organizer = new AppUser { Id = "org1", UserName = "org@test.com", Email = "org@test.com", DisplayName = "Org" };
        db.Users.Add(user);
        db.Users.Add(organizer);

        var ev = new Event
        {
            Title = "Test Event",
            Description = "A test event",
            EventDate = DateTime.UtcNow.AddDays(7),
            Time = "6:00 PM",
            Location = "City Hall",
            Category = EventCategory.Education,
            OrganizerId = "org1"
        };
        db.Events.Add(ev);
        await db.SaveChangesAsync();

        return (db, user.Id, ev.Id);
    }

    [Fact]
    public async Task AddBookmarkAsync_CreatesBookmark()
    {
        var (db, userId, eventId) = await SeedDataAsync();
        var service = new BookmarkService(db);

        var result = await service.AddBookmarkAsync(userId, eventId);

        Assert.True(result);
        Assert.Single(db.Bookmarks);
    }

    [Fact]
    public async Task AddBookmarkAsync_ReturnsFalse_WhenDuplicate()
    {
        var (db, userId, eventId) = await SeedDataAsync();
        var service = new BookmarkService(db);

        await service.AddBookmarkAsync(userId, eventId);
        var result = await service.AddBookmarkAsync(userId, eventId);

        Assert.False(result);
    }

    [Fact]
    public async Task AddBookmarkAsync_ReturnsFalse_WhenEventNotFound()
    {
        var (db, userId, _) = await SeedDataAsync();
        var service = new BookmarkService(db);

        var result = await service.AddBookmarkAsync(userId, 999);

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveBookmarkAsync_RemovesBookmark()
    {
        var (db, userId, eventId) = await SeedDataAsync();
        var service = new BookmarkService(db);

        await service.AddBookmarkAsync(userId, eventId);
        var result = await service.RemoveBookmarkAsync(userId, eventId);

        Assert.True(result);
        Assert.Empty(db.Bookmarks);
    }

    [Fact]
    public async Task RemoveBookmarkAsync_ReturnsFalse_WhenNotBookmarked()
    {
        var (db, userId, eventId) = await SeedDataAsync();
        var service = new BookmarkService(db);

        var result = await service.RemoveBookmarkAsync(userId, eventId);

        Assert.False(result);
    }

    [Fact]
    public async Task GetUserBookmarksAsync_ReturnsBookmarkedEvents()
    {
        var (db, userId, eventId) = await SeedDataAsync();
        var service = new BookmarkService(db);

        await service.AddBookmarkAsync(userId, eventId);
        var bookmarks = await service.GetUserBookmarksAsync(userId);

        Assert.Single(bookmarks);
        Assert.Equal("Test Event", bookmarks[0].Title);
        Assert.True(bookmarks[0].IsBookmarked);
    }

    [Fact]
    public async Task GetUserBookmarksAsync_ReturnsEmpty_WhenNoBookmarks()
    {
        var (db, userId, _) = await SeedDataAsync();
        var service = new BookmarkService(db);

        var bookmarks = await service.GetUserBookmarksAsync(userId);

        Assert.Empty(bookmarks);
    }
}
