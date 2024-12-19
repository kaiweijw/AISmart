using System.Collections.Generic;
using System.Threading.Tasks;
using AISmart.Options;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using Volo.Abp.DependencyInjection;

namespace AISmart.Provider;

public class ChatAgentProvider : IChatAgentProvider, ITransientDependency
{
    private readonly ILogger<ChatAgentProvider> _logger;
    private readonly MicroAIOptions _options;

    private readonly Dictionary<string, MiddlewareStreamingAgent<OpenAIChatAgent>> _agents = new();

    public ChatAgentProvider(IOptions<MicroAIOptions> options,ILogger<ChatAgentProvider> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    
    public async Task<IMessage?> SendAsync(string agentName, string message, IEnumerable<IMessage>? chatHistory)
    {
        if (_agents.TryGetValue(agentName, out var middlewareAgent) == true)
        {
            return await middlewareAgent.SendAsync(message, chatHistory);   
        }
        
        _logger.LogWarning($"[ChatAgentProvider] {agentName} not exist");
        return null;
    }

    public void SetAgent(string agentName, string systemMessage)
    {
        var client = new ChatClient(_options.Model, _options.ApiKey);
        var agent = new OpenAIChatAgent(client, agentName, systemMessage)
            .RegisterMessageConnector();
          
        _agents.Add(agentName, agent);
    }
}