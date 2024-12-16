using AISmart.GAgent.Autogen.Options;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using Volo.Abp.DependencyInjection;

namespace AISmart.GAgent.Autogen;

public class ChatAgentProvider : IChatAgentProvider, ITransientDependency
{
    private readonly AutogenOptions _options;

    private readonly Dictionary<string, MiddlewareAgent<MiddlewareStreamingAgent<OpenAIChatAgent>>> _agents = new();

    public ChatAgentProvider(IOptions<AutogenOptions> options)
    {
        _options = options.Value;
    }

    public MiddlewareAgent<MiddlewareStreamingAgent<OpenAIChatAgent>> GetAgent(string agentName)
    {
        _agents.TryGetValue(agentName, out var agent);
        return agent;
    }

    public void SetAgent(string agentName, string systemMessage, FunctionCallMiddleware middleware)
    {
        var client = new ChatClient(_options.Model, _options.ApiKey);
        var agent = new OpenAIChatAgent(client, agentName, systemMessage).RegisterMessageConnector()
            .RegisterMessageConnector()
            .RegisterMiddleware(middleware);
        _agents.Add(agentName, agent);
    }
}