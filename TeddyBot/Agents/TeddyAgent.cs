using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using TeddyBot.Configuration;
using TeddyBot.Services;

namespace TeddyBot.Agents;

public sealed class TeddyAgent : ITeddyAgent
{
    private readonly ChatCompletionAgent _agent;
    private readonly ConversationContext _context;

    public string Name => _agent.Name ?? "TeddyBot";

    public TeddyAgent(
        Kernel kernel,
        ConversationContext context,
        IOptions<TeddyBotSettings> settings)
    {
        _context = context;

        _agent = new ChatCompletionAgent
        {
            Name = settings.Value.AgentName,
            Instructions = settings.Value.SystemPrompt,
            Kernel = kernel,
            Arguments = new KernelArguments(
                new AzureOpenAIPromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                })
        };
    }

    public async IAsyncEnumerable<string> InvokeStreamingAsync(
        string userMessage,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var message = new ChatMessageContent(AuthorRole.User, userMessage);

        await foreach (var response in _agent.InvokeStreamingAsync(
                           message,
                           _context.Thread,
                           cancellationToken: cancellationToken))
        {
            if (response.Message.Content is not null)
            {
                yield return response.Message.Content;
            }
        }
    }
}
