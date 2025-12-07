namespace TeddyBot.Services;

public interface IChatService
{
    Task<string> SendMessageAsync(string message, CancellationToken cancellationToken = default);

    IAsyncEnumerable<string> SendMessageStreamingAsync(
        string message,
        CancellationToken cancellationToken = default);

    void ResetConversation();
}
