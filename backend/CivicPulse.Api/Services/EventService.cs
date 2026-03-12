using CivicPulse.Api.Data;
using CivicPulse.Api.Dtos;
using CivicPulse.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CivicPulse.Api.Services;

public class EventService : IEventService
{
    private readonly AppDbContext _db;
    private readonly ISummaryService _summaryService;
    private readonly IEmbeddingService _embeddingService;
    private readonly ISearchService _searchService;
    private readonly ILogger<EventService> _logger;

    public EventService(AppDbContext db, ISummaryService summaryService,
        IEmbeddingService embeddingService, ISearchService searchService,
        ILogger<EventService> logger)
    {
        _db = db;
        _summaryService = summaryService;
        _embeddingService = embeddingService;
        _searchService = searchService;
        _logger = logger;
    }

    public async Task<EventDto> CreateEventAsync(EventCreateDto dto, string organizerId)
    {
        var ev = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            EventDate = dto.EventDate,
            Time = dto.Time,
            Location = dto.Location,
            Category = dto.Category,
            OrganizerId = organizerId
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();

        // Generate AI summary
        try
        {
            var (summary, whyItMatters) = await _summaryService.GenerateSummaryAsync(
                dto.Title, dto.Description, dto.Category.ToString());
            ev.AiSummary = summary;
            ev.WhyItMatters = whyItMatters;
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to generate AI summary for event {EventId}", ev.Id);
        }

        // Generate embedding and index in search
        try
        {
            var textToEmbed = $"{ev.Title} {ev.Description} {ev.Category} {ev.AiSummary}";
            var vector = await _embeddingService.VectorizeTextAsync(textToEmbed);
            await _searchService.IndexEventAsync(
                ev.Id, ev.Title, ev.Description, ev.AiSummary,
                ev.Category.ToString(), ev.EventDate, ev.Location,
                ev.OrganizerId, vector);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to index event {EventId} in search", ev.Id);
        }

        return await MapToDto(ev, null);
    }

    public async Task<EventDto?> GetEventByIdAsync(int id, string? userId = null)
    {
        var ev = await _db.Events
            .Include(e => e.Organizer)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null) return null;
        return await MapToDto(ev, userId);
    }

    public async Task<List<EventDto>> GetEventsAsync(EventCategory? category = null, int page = 1, int pageSize = 20, string? userId = null)
    {
        var query = _db.Events
            .Include(e => e.Organizer)
            .Where(e => e.IsApproved)
            .AsQueryable();

        if (category.HasValue)
            query = query.Where(e => e.Category == category.Value);

        var events = await query
            .OrderByDescending(e => e.EventDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = new List<EventDto>();
        foreach (var ev in events)
            dtos.Add(await MapToDto(ev, userId));
        return dtos;
    }

    public async Task<EventDto?> UpdateEventAsync(int id, EventUpdateDto dto, string userId, bool isAdmin)
    {
        var ev = await _db.Events.Include(e => e.Organizer).FirstOrDefaultAsync(e => e.Id == id);
        if (ev == null) return null;
        if (ev.OrganizerId != userId && !isAdmin) return null;

        if (dto.Title != null) ev.Title = dto.Title;
        if (dto.Description != null) ev.Description = dto.Description;
        if (dto.EventDate.HasValue) ev.EventDate = dto.EventDate.Value;
        if (dto.Time != null) ev.Time = dto.Time;
        if (dto.Location != null) ev.Location = dto.Location;
        if (dto.Category.HasValue) ev.Category = dto.Category.Value;
        ev.UpdatedAt = DateTime.UtcNow;

        // Regenerate AI summary if title or description changed
        if (dto.Title != null || dto.Description != null)
        {
            try
            {
                var (summary, whyItMatters) = await _summaryService.GenerateSummaryAsync(
                    ev.Title, ev.Description, ev.Category.ToString());
                ev.AiSummary = summary;
                ev.WhyItMatters = whyItMatters;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to regenerate AI summary for event {EventId}", ev.Id);
            }
        }

        await _db.SaveChangesAsync();

        // Re-index in search
        try
        {
            var textToEmbed = $"{ev.Title} {ev.Description} {ev.Category} {ev.AiSummary}";
            var vector = await _embeddingService.VectorizeTextAsync(textToEmbed);
            await _searchService.IndexEventAsync(
                ev.Id, ev.Title, ev.Description, ev.AiSummary,
                ev.Category.ToString(), ev.EventDate, ev.Location,
                ev.OrganizerId, vector);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to re-index event {EventId}", ev.Id);
        }

        return await MapToDto(ev, userId);
    }

    public async Task<bool> DeleteEventAsync(int id, string userId, bool isAdmin)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev == null) return false;
        if (ev.OrganizerId != userId && !isAdmin) return false;

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();

        try
        {
            await _searchService.DeleteEventAsync(id.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to remove event {EventId} from search index", id);
        }

        return true;
    }

    public async Task<List<EventDto>> GetEventsByOrganizerAsync(string organizerId)
    {
        var events = await _db.Events
            .Include(e => e.Organizer)
            .Where(e => e.OrganizerId == organizerId)
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();

        var dtos = new List<EventDto>();
        foreach (var ev in events)
            dtos.Add(await MapToDto(ev, organizerId));
        return dtos;
    }

    private async Task<EventDto> MapToDto(Event ev, string? userId)
    {
        var isBookmarked = false;
        if (userId != null)
        {
            isBookmarked = await _db.Bookmarks
                .AnyAsync(b => b.UserId == userId && b.EventId == ev.Id);
        }

        return new EventDto
        {
            Id = ev.Id,
            Title = ev.Title,
            Description = ev.Description,
            EventDate = ev.EventDate,
            Time = ev.Time,
            Location = ev.Location,
            Category = ev.Category.ToString(),
            AiSummary = ev.AiSummary,
            WhyItMatters = ev.WhyItMatters,
            OrganizerName = ev.Organizer?.DisplayName ?? "",
            OrganizerId = ev.OrganizerId,
            IsBookmarked = isBookmarked,
            IsApproved = ev.IsApproved
        };
    }
}
