namespace CivicPulse.Api.Configuration;

public class AzureAISearchConfig
{
    public string Endpoint { get; set; } = string.Empty;
    public string AdminApiKey { get; set; } = string.Empty;
    public string IndexName { get; set; } = "events-index";
}

public class AzureAIVisionConfig
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}

public class OpenAIConfig
{
    public string ApiKey { get; set; } = string.Empty;
    public string ChatModel { get; set; } = "gpt-4o-mini";
}

public class JwtConfig
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "CivicPulse";
    public string Audience { get; set; } = "CivicPulse";
    public int ExpiryMinutes { get; set; } = 1440;
}
