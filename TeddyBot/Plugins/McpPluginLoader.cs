using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using ModelContextProtocol.Client;
using TeddyBot.Configuration;

namespace TeddyBot.Plugins;

public sealed class McpPluginLoader : IAsyncDisposable
{
    private readonly Kernel _kernel;
    private readonly McpSettings _settings;
    private readonly ILogger<McpPluginLoader> _logger;
    private readonly List<McpClient> _clients = [];

    public McpPluginLoader(
        Kernel kernel,
        IOptions<McpSettings> settings,
        ILogger<McpPluginLoader> logger)
    {
        _kernel = kernel;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        foreach (var serverConfig in _settings.Servers.Where(s => s.Enabled))
        {
            try
            {
                _logger.LogInformation("Connecting to MCP server: {Name}", serverConfig.Name);

                var clientTransport = new StdioClientTransport(new()
                {
                    Name = serverConfig.Name,
                    Command = serverConfig.Command,
                    Arguments = [.. serverConfig.Arguments]
                });

                var client = await McpClient.CreateAsync(
                    clientTransport,
                    cancellationToken: cancellationToken);

                _clients.Add(client);

                // Get tools and add to kernel
                var tools = await client.ListToolsAsync();

                if (tools.Count > 0)
                {
                    var functions = tools.Select(tool => tool.AsKernelFunction());
                    _kernel.Plugins.AddFromFunctions(serverConfig.Name, functions);

                    _logger.LogInformation(
                        "Loaded {Count} tools from MCP server: {Name}",
                        tools.Count,
                        serverConfig.Name);
                }
                else
                {
                    _logger.LogWarning("No tools found in MCP server: {Name}", serverConfig.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load MCP server: {Name}", serverConfig.Name);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var client in _clients)
        {
            if (client is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else if (client is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        _clients.Clear();
    }
}
