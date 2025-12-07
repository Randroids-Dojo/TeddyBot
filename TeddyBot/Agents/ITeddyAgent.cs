namespace TeddyBot.Agents;

public interface ITeddyAgent
{
    string Name { get; }

    IAsyncEnumerable<string> InvokeStreamingAsync(
        string userMessage,
        CancellationToken cancellationToken = default);
}
