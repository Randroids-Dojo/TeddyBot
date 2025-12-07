namespace TeddyBot.Configuration;

public sealed class AzureOpenAISettings
{
    public const string SectionName = "AzureOpenAI";

    public required string Endpoint { get; init; }
    public required string DeploymentName { get; init; }
    public string? ApiKey { get; init; }
    public string? ModelId { get; init; }
}
