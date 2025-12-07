# TeddyBot

An AI console application built with Semantic Kernel and Microsoft.Extensions.AI, featuring MCP (Model Context Protocol) tool integration.

## Features

- **Conversational AI** - Chat with context maintained across turns
- **Tool Calling** - Automatic function invocation via MCP servers
- **File System Access** - Read, write, search, and manage files in allowed directories
- **Extensible Architecture** - Built for adding subagents, tools, and evaluation

## Tech Stack

| Component | Technology |
|-----------|------------|
| Framework | .NET 10.0 |
| AI Orchestration | [Semantic Kernel](https://github.com/microsoft/semantic-kernel) 1.68.0 |
| Abstractions | [Microsoft.Extensions.AI](https://github.com/dotnet/extensions) 10.0.1 |
| Tool Protocol | [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) |
| LLM Provider | Azure OpenAI (gpt-4.1-mini) |
| Hosting | Microsoft.Extensions.Hosting |

## Project Structure

```
TeddyBot/
├── Program.cs                      # Entry point with DI host setup
├── appsettings.json                # Configuration
├── Configuration/
│   ├── AzureOpenAISettings.cs      # Azure OpenAI connection settings
│   ├── TeddyBotSettings.cs         # Agent name and system prompt
│   └── McpSettings.cs              # MCP server configuration
├── Agents/
│   ├── ITeddyAgent.cs              # Agent abstraction
│   └── TeddyAgent.cs               # ChatCompletionAgent wrapper
├── Services/
│   ├── IChatService.cs             # Chat orchestration interface
│   ├── ChatService.cs              # Message handling
│   └── ConversationContext.cs      # Conversation state
├── Plugins/
│   └── McpPluginLoader.cs          # MCP tool loader
├── Hosting/
│   └── TeddyBotHostedService.cs    # Console interaction loop
└── workspace/                      # Directory for file operations
```

## Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (for MCP filesystem server via npx)
- Azure OpenAI resource or Azure AI Foundry project

### Configuration

1. Set your Azure OpenAI credentials:

```bash
cd TeddyBot
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://your-resource.cognitiveservices.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "your-api-key"
```

2. Update `appsettings.json` if using a different model deployment:

```json
"AzureOpenAI": {
  "DeploymentName": "your-deployment-name",
  "ModelId": "your-model-id"
}
```

### Running

```bash
cd TeddyBot
dotnet run
```

Commands:
- Type your message and press Enter to chat
- `clear` - Reset conversation history
- `exit` - Quit the application

### Example Usage

```
You: What can you do?
TeddyBot: I can assist you with questions, writing, and file operations...

You: Create a file called notes.txt with "Meeting at 3pm"
TeddyBot: I have created notes.txt with your content.

You: List files in workspace
TeddyBot: There is one file: notes.txt
```

## Available MCP Tools

The filesystem MCP server provides 14 tools:

| Tool | Description |
|------|-------------|
| `read_file` | Read contents of a file |
| `write_file` | Create or overwrite a file |
| `read_multiple_files` | Read multiple files at once |
| `create_directory` | Create a new directory |
| `list_directory` | List directory contents |
| `move_file` | Move or rename files |
| `search_files` | Search for files by pattern |
| `get_file_info` | Get file metadata |
| `list_allowed_directories` | Show accessible directories |
| And more... | |

## Roadmap

### Planned Features

- [ ] **Multiple Subagents** - Specialized agents for different tasks (coding, research, etc.)
- [ ] **Additional MCP Servers** - GitHub, SQLite, web browsing, and custom tools
- [ ] **Custom Plugins** - Native C# plugins with `[KernelFunction]` attributes
- [ ] **Agent Evaluation** - Microsoft.Extensions.AI.Evaluation integration for measuring performance
- [ ] **Prompt Iteration** - Data-driven prompt optimization based on evaluation metrics
- [ ] **Memory/RAG** - Long-term memory and retrieval-augmented generation
- [ ] **Multi-turn Planning** - Complex task decomposition and execution

### Future Integrations

- [ ] Azure AI Foundry Agents
- [ ] OpenTelemetry observability
- [ ] Structured output / JSON mode
- [ ] Image and multimodal support

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    TeddyBotHostedService                │
│                    (Console Loop)                       │
└─────────────────────┬───────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────┐
│                      ChatService                        │
│                 (Message Orchestration)                 │
└─────────────────────┬───────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────┐
│                      TeddyAgent                         │
│              (ChatCompletionAgent Wrapper)              │
│  ┌─────────────────────────────────────────────────┐   │
│  │            Semantic Kernel                       │   │
│  │  ┌─────────────┐  ┌──────────────────────────┐  │   │
│  │  │ Azure OpenAI│  │     MCP Plugins          │  │   │
│  │  │   Service   │  │  (filesystem, etc.)      │  │   │
│  │  └─────────────┘  └──────────────────────────┘  │   │
│  └─────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

## Adding MCP Servers

Edit `appsettings.json` to add more MCP servers:

```json
"Mcp": {
  "Servers": [
    {
      "Name": "filesystem",
      "Command": "npx",
      "Arguments": ["-y", "@modelcontextprotocol/server-filesystem", "./workspace"],
      "Enabled": true
    },
    {
      "Name": "github",
      "Command": "npx",
      "Arguments": ["-y", "@modelcontextprotocol/server-github"],
      "Enabled": true
    }
  ]
}
```

## License

MIT

## Resources

- [Semantic Kernel Documentation](https://learn.microsoft.com/semantic-kernel/)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai)
- [Model Context Protocol](https://modelcontextprotocol.io/)
- [Azure AI Foundry](https://ai.azure.com/)
