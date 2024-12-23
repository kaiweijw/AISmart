using AISmart.GAgent.Autogen.Options;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using Google.Apis.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using Volo.Abp.DependencyInjection;

namespace AISmart.GAgent.Autogen;

public class ChatAgentProvider : IChatAgentProvider, ITransientDependency
{
    private readonly ILogger<ChatAgentProvider> _logger;
    private readonly AutogenOptions _options;


    public ChatAgentProvider(IOptions<AutogenOptions> options,ILogger<ChatAgentProvider> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    
    public async Task<?> SendAsync(string agentName, string message,)
    {
        if (_agents.TryGetValue(agentName, out var middlewareAgent) == true)
        {
            return await middlewareAgent.SendAsync(message, chatHistory);   
        }
        
        _logger.LogWarning($"[ChatAgentProvider] {agentName} not exist");
        return null;
    }
}