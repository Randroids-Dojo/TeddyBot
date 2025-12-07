using Microsoft.SemanticKernel.Agents;

namespace TeddyBot.Services;

public sealed class ConversationContext
{
    public ChatHistoryAgentThread Thread { get; private set; } = new();

    public void Reset() => Thread = new();
}
