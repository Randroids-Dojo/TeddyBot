using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeddyBot.Services;

namespace TeddyBot.Hosting;

public sealed class TeddyBotHostedService(
    IChatService chatService,
    ILogger<TeddyBotHostedService> logger,
    IHostApplicationLifetime lifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine();
        Console.WriteLine("TeddyBot is ready! Type 'exit' to quit, 'clear' to reset conversation.");
        Console.WriteLine();

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.Write("You: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                lifetime.StopApplication();
                break;
            }

            if (input.Equals("clear", StringComparison.OrdinalIgnoreCase))
            {
                chatService.ResetConversation();
                Console.WriteLine("Conversation cleared.");
                Console.WriteLine();
                continue;
            }

            Console.Write("TeddyBot: ");

            try
            {
                await foreach (var chunk in chatService.SendMessageStreamingAsync(input, stoppingToken))
                {
                    Console.Write(chunk);
                }

                Console.WriteLine();
                Console.WriteLine();
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing message");
                Console.WriteLine();
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine();
            }
        }
    }
}
