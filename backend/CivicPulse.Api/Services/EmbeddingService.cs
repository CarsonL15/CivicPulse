using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CivicPulse.Api.Configuration;
using Microsoft.Extensions.Options;

namespace CivicPulse.Api.Services;

public class EmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;
    private const string ApiVersion = "2024-02-01";
    private const string ModelVersion = "2023-04-15";

    public EmbeddingService(IOptions<AzureAIVisionConfig> config, HttpClient httpClient)
    {
        _endpoint = config.Value.Endpoint.TrimEnd('/');
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", config.Value.ApiKey);
    }

    public async Task<float[]> VectorizeTextAsync(string text)
    {
        var url = $"{_endpoint}/computervision/retrieval:vectorizeText?api-version={ApiVersion}&model-version={ModelVersion}";
        var payload = JsonSerializer.Serialize(new { text });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("vector").EnumerateArray()
            .Select(v => v.GetSingle()).ToArray();
    }

    public async Task<float[]> VectorizeImageAsync(byte[] imageData)
    {
        var url = $"{_endpoint}/computervision/retrieval:vectorizeImage?api-version={ApiVersion}&model-version={ModelVersion}";
        var content = new ByteArrayContent(imageData);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("vector").EnumerateArray()
            .Select(v => v.GetSingle()).ToArray();
    }
}
