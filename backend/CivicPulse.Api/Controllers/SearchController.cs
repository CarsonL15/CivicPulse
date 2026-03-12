using CivicPulse.Api.Dtos;
using CivicPulse.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CivicPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly IEmbeddingService _embeddingService;

    public SearchController(ISearchService searchService, IEmbeddingService embeddingService)
    {
        _searchService = searchService;
        _embeddingService = embeddingService;
    }

    /// <summary>
    /// Text-based hybrid search (vector + full-text + semantic ranking)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<EventSearchResultDto>>> Search(
        [FromQuery] string q = "",
        [FromQuery] string? category = null)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Ok(new List<EventSearchResultDto>());

        var queryVector = await _embeddingService.VectorizeTextAsync(q);
        var results = await _searchService.HybridSearchAsync(q, queryVector, category);
        return Ok(results);
    }

    /// <summary>
    /// Image-based search — upload an event poster to find matching events
    /// </summary>
    [HttpPost("image")]
    public async Task<ActionResult<List<EventSearchResultDto>>> SearchByImage(
        IFormFile image,
        [FromQuery] string? category = null)
    {
        if (image == null || image.Length == 0)
            return BadRequest("No image provided");

        using var ms = new MemoryStream();
        await image.CopyToAsync(ms);
        var imageBytes = ms.ToArray();

        var imageVector = await _embeddingService.VectorizeImageAsync(imageBytes);
        // Image search uses vector only (no query text for full-text matching)
        var results = await _searchService.HybridSearchAsync("", imageVector, category);
        return Ok(results);
    }
}
