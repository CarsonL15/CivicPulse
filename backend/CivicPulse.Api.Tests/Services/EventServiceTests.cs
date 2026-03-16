using CivicPulse.Api.Data;
using CivicPulse.Api.Dtos;
using CivicPulse.Api.Models;
using CivicPulse.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CivicPulse.Api.Tests.Services;

public class EventServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static EventService CreateService(AppDbContext db)
    {
        var summaryService = new Mock<ISummaryService>();
        summaryService.Setup(s => s.GenerateSummaryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(("Test summary", "This matters because..."));

        var embeddingService = new Mock<IEmbeddingService>();
        embeddingService.Setup(s => s.VectorizeTextAsync(It.IsAny<string>()))
            .ReturnsAsync(new float[1024]);

        var searchService = new Mock<ISearchService>();
        var logger = new Mock<ILogger<EventService>>();

        return new EventService(db, summaryService.Object, embeddingService.Object,
            searchService.Object, logger.Object);
    }

    private static async Task SeedOrganizerAsync(AppDbContext db)
    {
        db.Users.Add(new AppUser
        {
            Id = "org1",
            UserName = "org@test.com",
            Email = "org@test.com",
            DisplayName = "Test Organizer"
        });
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task CreateEventAsync_CreatesEventWithAiSummary()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db);
        var service = CreateService(db);

        var dto = new EventCreateDto
        {
            Title = "City Council Meeting",
            Description = "Discussion about new zoning laws",
            EventDate = DateTime.UtcNow.AddDays(7),
            Time = "6:00 PM - 8:00 PM",
            Location = "City Hall",
            Category = EventCategory.Housing
        };

        var result = await service.CreateEventAsync(dto, "org1");

        Assert.NotNull(result);
        Assert.Equal("City Council Meeting", result.Title);
        Assert.Equal("Test summary", result.AiSummary);
        Assert.Equal("This matters because...", result.WhyItMatters);
    }

    [Fact]
    public async Task GetEventByIdAsync_ReturnsNull_WhenNotFound()
    {
        var db = CreateContext();
        var service = CreateService(db);

        var result = await service.GetEventByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetEventsAsync_FiltersByCategory()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db);
        var service = CreateService(db);

        await service.CreateEventAsync(new EventCreateDto
        {
            Title = "Housing Event", Description = "Test", EventDate = DateTime.UtcNow.AddDays(7),
            Time = "6 PM", Location = "City Hall", Category = EventCategory.Housing
        }, "org1");

        await service.CreateEventAsync(new EventCreateDto
        {
            Title = "Education Event", Description = "Test", EventDate = DateTime.UtcNow.AddDays(7),
            Time = "6 PM", Location = "School", Category = EventCategory.Education
        }, "org1");

        var housingEvents = await service.GetEventsAsync(EventCategory.Housing);
        var educationEvents = await service.GetEventsAsync(EventCategory.Education);

        Assert.Single(housingEvents);
        Assert.Equal("Housing Event", housingEvents[0].Title);
        Assert.Single(educationEvents);
        Assert.Equal("Education Event", educationEvents[0].Title);
    }

    [Fact]
    public async Task DeleteEventAsync_ReturnsFalse_WhenNotOwner()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db);
        var service = CreateService(db);

        var ev = await service.CreateEventAsync(new EventCreateDto
        {
            Title = "Test", Description = "Test", EventDate = DateTime.UtcNow.AddDays(7),
            Time = "6 PM", Location = "Here", Category = EventCategory.Education
        }, "org1");

        var result = await service.DeleteEventAsync(ev.Id, "different-user", false);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteEventAsync_Succeeds_WhenAdmin()
    {
        var db = CreateContext();
        await SeedOrganizerAsync(db);
        var service = CreateService(db);

        var ev = await service.CreateEventAsync(new EventCreateDto
        {
            Title = "Test", Description = "Test", EventDate = DateTime.UtcNow.AddDays(7),
            Time = "6 PM", Location = "Here", Category = EventCategory.Education
        }, "org1");

        var result = await service.DeleteEventAsync(ev.Id, "different-user", true);

        Assert.True(result);
    }
}
