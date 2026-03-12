using System.Security.Claims;
using CivicPulse.Api.Dtos;
using CivicPulse.Api.Models;
using CivicPulse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CivicPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventDto>>> GetEvents(
        [FromQuery] EventCategory? category = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var events = await _eventService.GetEventsAsync(category, page, pageSize, userId);
        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EventDto>> GetEvent(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var ev = await _eventService.GetEventByIdAsync(id, userId);
        if (ev == null) return NotFound();
        return Ok(ev);
    }

    [HttpPost]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<ActionResult<EventDto>> CreateEvent(EventCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var ev = await _eventService.CreateEventAsync(dto, userId);
        return CreatedAtAction(nameof(GetEvent), new { id = ev.Id }, ev);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<ActionResult<EventDto>> UpdateEvent(int id, EventUpdateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");
        var ev = await _eventService.UpdateEventAsync(id, dto, userId, isAdmin);
        if (ev == null) return NotFound();
        return Ok(ev);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<ActionResult> DeleteEvent(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");
        var result = await _eventService.DeleteEventAsync(id, userId, isAdmin);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpGet("my-events")]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<ActionResult<List<EventDto>>> GetMyEvents()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var events = await _eventService.GetEventsByOrganizerAsync(userId);
        return Ok(events);
    }
}
