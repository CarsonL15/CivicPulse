using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using CivicPulse.Api.Configuration;
using CivicPulse.Api.Dtos;
using CivicPulse.Api.SearchModels;
using Microsoft.Extensions.Options;

namespace CivicPulse.Api.Services;

public class SearchService : ISearchService
{
    private readonly SearchClient _searchClient;
    private readonly SearchIndexClient _indexClient;
    private readonly string _indexName;

    public SearchService(IOptions<AzureAISearchConfig> config)
    {
        var credential = new AzureKeyCredential(config.Value.AdminApiKey);
        _indexName = config.Value.IndexName;
        _indexClient = new SearchIndexClient(new Uri(config.Value.Endpoint), credential);
        _searchClient = _indexClient.GetSearchClient(_indexName);
    }

    public async Task EnsureIndexAsync()
    {
        var fields = new FieldBuilder().Build(typeof(EventSearchDocument));

        var index = new SearchIndex(_indexName, fields)
        {
            VectorSearch = new VectorSearch
            {
                Algorithms =
                {
                    new HnswAlgorithmConfiguration("hnsw-algorithm")
                    {
                        Parameters = new HnswParameters
                        {
                            M = 4,
                            EfConstruction = 400,
                            EfSearch = 500,
                            Metric = VectorSearchAlgorithmMetric.Cosine
                        }
                    }
                },
                Profiles =
                {
                    new VectorSearchProfile("vector-profile", "hnsw-algorithm")
                }
            },
            SemanticSearch = new SemanticSearch
            {
                Configurations =
                {
                    new SemanticConfiguration("semantic-config", new SemanticPrioritizedFields
                    {
                        TitleField = new SemanticField("title"),
                        ContentFields = { new SemanticField("description"), new SemanticField("aiSummary") }
                    })
                }
            }
        };

        try
        {
            await _indexClient.DeleteIndexAsync(_indexName);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            // Index doesn't exist yet
        }

        await _indexClient.CreateIndexAsync(index);
    }

    public async Task IndexEventAsync(int eventId, string title, string description, string? aiSummary,
        string category, DateTimeOffset eventDate, string location, string organizerId, float[] contentVector)
    {
        var doc = new EventSearchDocument
        {
            Id = eventId.ToString(),
            Title = title,
            Description = description,
            AiSummary = aiSummary,
            Category = category,
            EventDate = eventDate,
            Location = location,
            OrganizerId = organizerId,
            ContentVector = contentVector
        };

        var batch = IndexDocumentsBatch.Upload(new[] { doc });
        await _searchClient.IndexDocumentsAsync(batch);
    }

    public async Task DeleteEventAsync(string eventId)
    {
        var batch = IndexDocumentsBatch.Delete("id", new[] { eventId });
        await _searchClient.IndexDocumentsAsync(batch);
    }

    public async Task<List<EventSearchResultDto>> HybridSearchAsync(string queryText, float[] queryVector, string? categoryFilter = null)
    {
        var options = new SearchOptions
        {
            Size = 20,
            QueryType = SearchQueryType.Semantic,
            SemanticSearch = new SemanticSearchOptions
            {
                SemanticConfigurationName = "semantic-config",
            },
            Select = { "id", "title", "aiSummary", "category", "eventDate", "location" },
            VectorSearch = new VectorSearchOptions
            {
                Queries =
                {
                    new VectorizedQuery(queryVector)
                    {
                        KNearestNeighborsCount = 20,
                        Fields = { "contentVector" }
                    }
                }
            }
        };

        if (!string.IsNullOrEmpty(categoryFilter))
        {
            options.Filter = $"category eq '{categoryFilter}'";
        }

        var response = await _searchClient.SearchAsync<EventSearchDocument>(queryText, options);

        var results = new List<EventSearchResultDto>();
        await foreach (var result in response.Value.GetResultsAsync())
        {
            var doc = result.Document;
            results.Add(new EventSearchResultDto
            {
                Id = int.Parse(doc.Id),
                Title = doc.Title,
                AiSummary = doc.AiSummary,
                Category = doc.Category,
                EventDate = doc.EventDate.DateTime,
                Location = doc.Location,
                Score = result.Score ?? 0
            });
        }

        return results;
    }
}
