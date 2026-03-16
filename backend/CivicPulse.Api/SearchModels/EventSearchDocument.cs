using System.Text.Json.Serialization;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace CivicPulse.Api.SearchModels;

public class EventSearchDocument
{
    [SimpleField(IsKey = true, IsFilterable = true)]
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [SearchableField(IsFilterable = true, IsSortable = true)]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [SearchableField(AnalyzerName = "en.microsoft")]
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [SearchableField(AnalyzerName = "en.microsoft")]
    [JsonPropertyName("aiSummary")]
    public string? AiSummary { get; set; }

    [SimpleField(IsFilterable = true, IsFacetable = true)]
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [SimpleField(IsFilterable = true, IsSortable = true)]
    [JsonPropertyName("eventDate")]
    public DateTimeOffset EventDate { get; set; }

    [SimpleField(IsFilterable = true)]
    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [SimpleField(IsFilterable = true)]
    [JsonPropertyName("organizerId")]
    public string OrganizerId { get; set; } = string.Empty;

    [VectorSearchField(VectorSearchDimensions = 1024, VectorSearchProfileName = "vector-profile")]
    [JsonPropertyName("contentVector")]
    public float[]? ContentVector { get; set; }
}
