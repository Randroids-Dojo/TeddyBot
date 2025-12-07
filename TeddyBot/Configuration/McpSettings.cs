namespace TeddyBot.Configuration;

public sealed class McpSettings
{
    public const string SectionName = "Mcp";

    public List<McpServerConfig> Servers { get; init; } = [];
}

public sealed class McpServerConfig
{
    public required string Name { get; init; }
    public required string Command { get; init; }
    public List<string> Arguments { get; init; } = [];
    public bool Enabled { get; init; } = true;
}
