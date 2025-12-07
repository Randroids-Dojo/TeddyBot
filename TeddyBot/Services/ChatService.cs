using System.Runtime.CompilerServices;
using System.Text;
using TeddyBot.Agents;

namespace TeddyBot.Services;

public sealed class ChatService(ITeddyAgent agent, ConversationContext context) : IChatService
{
    public async Task<string> SendMessageAsync(
        string message,
        CancellationToken cancellationToken = default)
    {
        var response = new StringBuilder();

        await foreach (var chunk in SendMessageStreamingAsync(message, cancellationToken))
        {
            response.Append(chunk);
        }

        return response.ToString();
    }

    public async IAsyncEnumerable<string> SendMessageStreamingAsync(
        string message,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var chunk in agent.InvokeStreamingAsync(message, cancellationToken))
        {
            yield return chunk;
        }
    }

    public void ResetConversation() => context.Reset();
}
