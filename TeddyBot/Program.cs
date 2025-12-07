using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using TeddyBot.Agents;
using TeddyBot.Configuration;
using TeddyBot.Hosting;
using TeddyBot.Plugins;
using TeddyBot.Services;

var builder = Host.CreateApplicationBuilder(args);

// Configuration
builder.Configuration.AddUserSecrets<Program>();

builder.Services.Configure<TeddyBotSettings>(
    builder.Configuration.GetSection(TeddyBotSettings.SectionName));
builder.Services.Configure<AzureOpenAISettings>(
    builder.Configuration.GetSection(AzureOpenAISettings.SectionName));
builder.Services.Configure<McpSettings>(
    builder.Configuration.GetSection(McpSettings.SectionName));

// Semantic Kernel
builder.Services.AddSingleton<Kernel>(sp =>
{
    var settings = builder.Configuration
        .GetSection(AzureOpenAISettings.SectionName)
        .Get<AzureOpenAISettings>()!;

    var kernelBuilder = Kernel.CreateBuilder();

    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: settings.DeploymentName,
        endpoint: settings.Endpoint,
        apiKey: settings.ApiKey ?? throw new InvalidOperationException(
            "Azure OpenAI API key is required. Set it via user-secrets: " +
            "dotnet user-secrets set \"AzureOpenAI:ApiKey\" \"your-key\""));

    return kernelBuilder.Build();
});

// MCP Plugin Loader
builder.Services.AddSingleton<McpPluginLoader>();

// Services
builder.Services.AddSingleton<ConversationContext>();
builder.Services.AddSingleton<ITeddyAgent, TeddyAgent>();
builder.Services.AddSingleton<IChatService, ChatService>();

// Hosted Service (Console Loop)
builder.Services.AddHostedService<TeddyBotHostedService>();

var host = builder.Build();

// Load MCP plugins before starting
var pluginLoader = host.Services.GetRequiredService<McpPluginLoader>();
await pluginLoader.LoadAsync(CancellationToken.None);

await host.RunAsync();
