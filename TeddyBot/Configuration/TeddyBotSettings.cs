namespace TeddyBot.Configuration;

public sealed class TeddyBotSettings
{
    public const string SectionName = "TeddyBot";

    public string AgentName { get; init; } = "TeddyBot";
    public string SystemPrompt { get; init; } = "You are TeddyBot, a helpful AI assistant.";
}
