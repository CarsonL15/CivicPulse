using CivicPulse.Api.Configuration;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace CivicPulse.Api.Services;

public class SummaryService : ISummaryService
{
    private readonly ChatClient _client;

    public SummaryService(IOptions<OpenAIConfig> config)
    {
        var openAiClient = new OpenAIClient(config.Value.ApiKey);
        _client = openAiClient.GetChatClient(config.Value.ChatModel);
    }

    public async Task<(string Summary, string WhyItMatters)> GenerateSummaryAsync(string title, string description, string category)
    {
        var systemPrompt = """
            You are a civic engagement assistant. Given a civic event's title, description, and category,
            produce two sections:

            1. SUMMARY: A plain-language summary (2-3 sentences) that explains what the event is about
               in simple terms anyone can understand. Avoid jargon and government-speak.

            2. WHY IT MATTERS: A brief explanation (1-2 sentences) of how this topic impacts everyday
               people, especially young adults aged 18-30.

            Respond in exactly this format:
            SUMMARY: <your summary>
            WHY IT MATTERS: <your explanation>
            """;

        var userPrompt = $"Title: {title}\nDescription: {description}\nCategory: {category}";

        var response = await _client.CompleteChatAsync(
            [
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userPrompt)
            ]);

        var content = response.Value.Content[0].Text;
        return ParseResponse(content);
    }

    private static (string Summary, string WhyItMatters) ParseResponse(string response)
    {
        var summary = "";
        var whyItMatters = "";

        var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (line.StartsWith("SUMMARY:", StringComparison.OrdinalIgnoreCase))
                summary = line["SUMMARY:".Length..].Trim();
            else if (line.StartsWith("WHY IT MATTERS:", StringComparison.OrdinalIgnoreCase))
                whyItMatters = line["WHY IT MATTERS:".Length..].Trim();
        }

        if (string.IsNullOrEmpty(summary))
            summary = response.Trim();

        return (summary, whyItMatters);
    }
}
