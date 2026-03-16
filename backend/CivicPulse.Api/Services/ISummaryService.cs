namespace CivicPulse.Api.Services;

public interface ISummaryService
{
    Task<(string Summary, string WhyItMatters)> GenerateSummaryAsync(string title, string description, string category);
}
