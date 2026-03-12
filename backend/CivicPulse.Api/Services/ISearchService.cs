using CivicPulse.Api.Dtos;

namespace CivicPulse.Api.Services;

public interface ISearchService
{
    Task EnsureIndexAsync();
    Task IndexEventAsync(int eventId, string title, string description, string? aiSummary, string category, DateTimeOffset eventDate, string location, string organizerId, float[] contentVector);
    Task DeleteEventAsync(string eventId);
    Task<List<EventSearchResultDto>> HybridSearchAsync(string queryText, float[] queryVector, string? categoryFilter = null);
}
