using CivicPulse.Api.Dtos;
using CivicPulse.Api.Models;

namespace CivicPulse.Api.Services;

public interface IEventService
{
    Task<EventDto> CreateEventAsync(EventCreateDto dto, string organizerId);
    Task<EventDto?> GetEventByIdAsync(int id, string? userId = null);
    Task<List<EventDto>> GetEventsAsync(EventCategory? category = null, int page = 1, int pageSize = 20, string? userId = null);
    Task<EventDto?> UpdateEventAsync(int id, EventUpdateDto dto, string userId, bool isAdmin);
    Task<bool> DeleteEventAsync(int id, string userId, bool isAdmin);
    Task<List<EventDto>> GetEventsByOrganizerAsync(string organizerId);
}
