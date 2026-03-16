using System.Security.Claims;
using CivicPulse.Api.Dtos;
using CivicPulse.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CivicPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookmarksController : ControllerBase
{
    private readonly IBookmarkService _bookmarkService;

    public BookmarksController(IBookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventDto>>> GetBookmarks()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var bookmarks = await _bookmarkService.GetUserBookmarksAsync(userId);
        return Ok(bookmarks);
    }

    [HttpPost("{eventId}")]
    public async Task<ActionResult> AddBookmark(int eventId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _bookmarkService.AddBookmarkAsync(userId, eventId);
        if (!result) return BadRequest("Already bookmarked or event not found");
        return Ok();
    }

    [HttpDelete("{eventId}")]
    public async Task<ActionResult> RemoveBookmark(int eventId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _bookmarkService.RemoveBookmarkAsync(userId, eventId);
        if (!result) return NotFound();
        return NoContent();
    }
}
