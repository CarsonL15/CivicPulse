namespace CivicPulse.Api.Services;

public interface IEmbeddingService
{
    Task<float[]> VectorizeTextAsync(string text);
    Task<float[]> VectorizeImageAsync(byte[] imageData);
}
