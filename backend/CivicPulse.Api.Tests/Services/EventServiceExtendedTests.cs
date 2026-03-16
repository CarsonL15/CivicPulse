using CivicPulse.Api.Data;
using CivicPulse.Api.Dtos;
using CivicPulse.Api.Models;
using CivicPulse.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CivicPulse.Api.Tests.Services;

public class EventServiceExtendedTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static (EventService service, Mock<ISummaryService> summary, Mock<IEmbeddingService> embedding, Mock<ISearchService> search) CreateServiceWithMocks(AppDbContext db)
    {
        var summaryService = new Mock<ISummaryService>();
        summaryService.Setup(s => s.GenerateSummaryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(("Test summary", "This matters because..."));

        var embeddingService = new Mock<IEmbeddingService>();
        embeddingService.Setup(s => s.VectorizeTextAsync(It.IsAny<string>()))
            .ReturnsAsync(new float[1024]);

        var searchService = new Mock<ISearchService>();
        var logger = new Mock<ILogger<EventService>>();

        var service = new EventService(db, summaryService.Object, embeddingService.Object,
            searchService.Object, logger.Object);

        return (service, summaryService, embeddingService, searchService);
    }

    private static async Task SeedOrganizerAsync(AppDbContext db, string id = "org1")
    {
        db.Users.Add(new AppUser
        {
            Id = id,
            UserName = $"{id}@test.com",
            Email = $"{id}@test.com",
            DisplayName = "Test Organizer"
        });
        await db.SaveChangesAsync();
    }

    private static EventCreateDto MakeDto(string title = "Test Event", EventCategory category = EventCategory.Education)
    {
        return new EventCreateDto
        {
            Title = title,
            Description = "A test event description",
            EventDate = DateTime.UtcNow.AddDays(7),
            Time = "6:00 PM - 8:00 PM",
            Location = "City Hall",
            Category = category
        };
    }

    [Fact]
    public async Task GetEventsAsync_ReturnsOnlyApprovedEvents()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db);
        db.Events.Add(new Event
        {
            Title = "Approved", Description = "Test", EventDate = DateTime.UtcNow.AddDays(1),
            Time = "6 PM", Location = "Here", Category = EventCategory.Education,
            OrganizerId = "org1", IsApproved = true
        });
        db.Events.Add(new Event
        {
            Title = "Not Approved", Description = "Test", EventDate = DateTime.UtcNow.AddDays(2),
            Time = "6 PM", Location = "There", Category = EventCategory.Education,
            OrganizerId = "org1", IsApproved = false
        });
        await db.SaveChangesAsync();

        var (service, _, _, _) = CreateServiceWithMocks(db);
        var results = await service.GetEventsAsync();

        Assert.Single(results);
        Assert.Equal("Approved", results[0].Title);
    }

    [Fact]
    public async Task GetEventsAsync_PaginatesCorrectly()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db);

        for (int i = 0; i < 5; i++)
        {
            db.Events.Add(new Event
            {
                Title = $"Event {i}", Description = "Test", EventDate = DateTime.UtcNow.AddDays(i),
                Time = "6 PM", Location = "Here", Category = EventCategory.Education,
                OrganizerId = "org1", IsApproved = true
            });
        }
        await db.SaveChangesAsync();

        var (service, _, _, _) = CreateServiceWithMocks(db);

        var page1 = await service.GetEventsAsync(page: 1, pageSize: 2);
        var page2 = await service.GetEventsAsync(page: 2, pageSize: 2);
        var page3 = await service.GetEventsAsync(page: 3, pageSize: 2);

        Assert.Equal(2, page1.Count);
        Assert.Equal(2, page2.Count);
        Assert.Single(page3);
    }

    [Fact]
    public async Task GetEventsAsync_OrdersByDateDescending()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db);

        db.Events.Add(new Event
        {
            Title = "Older", Description = "Test", EventDate = DateTime.UtcNow.AddDays(1),
            Time = "6 PM", Location = "Here", Category = EventCategory.Education,
            OrganizerId = "org1", IsApproved = true
        });
        db.Events.Add(new Event
        {
            Title = "Newer", Description = "Test", EventDate = DateTime.UtcNow.AddDays(10),
            Time = "6 PM", Location = "Here", Category = EventCategory.Education,
            OrganizerId = "org1", IsApproved = true
        });
        await db.SaveChangesAsync();

        var (service, _, _, _) = CreateServiceWithMocks(db);
        var results = await service.GetEventsAsync();

        Assert.Equal("Newer", results[0].Title);
        Assert.Equal("Older", results[1].Title);
    }

    [Fact]
    public async Task UpdateEventAsync_UpdatesFieldsAndRegeneratesSummary()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db);
        var (service, summaryMock, _, _) = CreateServiceWithMocks(db);

        var created = await service.CreateEventAsync(MakeDto("Original Title"), "org1");

        var updated = await service.UpdateEventAsync(created.Id, new EventUpdateDto
        {
            Title = "Updated Title",
            Location = "New Location"
        }, "org1", false);

        Assert.NotNull(updated);
        Assert.Equal("Updated Title", updated!.Title);
        Assert.Equal("New Location", updated.Location);
        summaryMock.Verify(s => s.GenerateSummaryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public async Task UpdateEventAsync_ReturnsNull_WhenNotOwner()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db);
        var (service, _, _, _) = CreateServiceWithMocks(db);

        var created = await service.CreateEventAsync(MakeDto(), "org1");

        var result = await service.UpdateEventAsync(created.Id, new EventUpdateDto { Title = "Hack" }, "other-user", false);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateEventAsync_AllowsAdmin()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db);
        var (service, _, _, _) = CreateServiceWithMocks(db);

        var created = await service.CreateEventAsync(MakeDto(), "org1");

        var result = await service.UpdateEventAsync(created.Id, new EventUpdateDto { Title = "Admin Edit" }, "admin-user", true);

        Assert.NotNull(result);
        Assert.Equal("Admin Edit", result!.Title);
    }

    [Fact]
    public async Task DeleteEventAsync_ReturnsFalse_WhenNotFound()
    {
        var db = CreateContext();
        var (service, _, _, _) = CreateServiceWithMocks(db);

        var result = await service.DeleteEventAsync(999, "org1", false);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteEventAsync_RemovesFromSearchIndex()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db);
        var (service, _, _, searchMock) = CreateServiceWithMocks(db);

        var created = await service.CreateEventAsync(MakeDto(), "org1");
        await service.DeleteEventAsync(created.Id, "org1", false);

        searchMock.Verify(s => s.DeleteEventAsync(created.Id.ToString()), Times.Once);
    }

    [Fact]
    public async Task CreateEventAsync_CallsEmbeddingAndSearchServices()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db);
        var (service, _, embeddingMock, searchMock) = CreateServiceWithMocks(db);

        await service.CreateEventAsync(MakeDto(), "org1");

        embeddingMock.Verify(s => s.VectorizeTextAsync(It.IsAny<string>()), Times.Once);
        searchMock.Verify(s => s.IndexEventAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(),
            It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<float[]>()), Times.Once);
    }

    [Fact]
    public async Task GetEventsByOrganizerAsync_ReturnsOnlyOrganizerEvents()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db, "org1");
        await SeedOrganizerAsync(db, "org2");
        var (service, _, _, _) = CreateServiceWithMocks(db);

        await service.CreateEventAsync(MakeDto("Org1 Event"), "org1");
        await service.CreateEventAsync(MakeDto("Org2 Event"), "org2");

        var org1Events = await service.GetEventsByOrganizerAsync("org1");
        var org2Events = await service.GetEventsByOrganizerAsync("org2");

        Assert.Single(org1Events);
        Assert.Equal("Org1 Event", org1Events[0].Title);
        Assert.Single(org2Events);
        Assert.Equal("Org2 Event", org2Events[0].Title);
    }

    [Fact]
    public async Task GetEventByIdAsync_ReturnsCorrectEvent()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db);
        var (service, _, _, _) = CreateServiceWithMocks(db);

        var created = await service.CreateEventAsync(MakeDto("Specific Event"), "org1");
        var result = await service.GetEventByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal("Specific Event", result!.Title);
        Assert.Equal("Test summary", result.AiSummary);
    }

    [Fact]
    public async Task GetEventByIdAsync_IncludesBookmarkStatus()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db);
        db.Users.Add(new AppUser { Id = "user1", UserName = "u@t.com", Email = "u@t.com", DisplayName = "User" });
        await db.SaveChangesAsync();

        var (service, _, _, _) = CreateServiceWithMocks(db);
        var created = await service.CreateEventAsync(MakeDto(), "org1");

        db.Bookmarks.Add(new Bookmark { UserId = "user1", EventId = created.Id });
        await db.SaveChangesAsync();

        var withBookmark = await service.GetEventByIdAsync(created.Id, "user1");
        var withoutBookmark = await service.GetEventByIdAsync(created.Id, "org1");

        Assert.True(withBookmark!.IsBookmarked);
        Assert.False(withoutBookmark!.IsBookmarked);
    }
}
