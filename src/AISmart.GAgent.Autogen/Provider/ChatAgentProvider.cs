using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using OpenAI.Chat;
using Volo.Abp.DependencyInjection;

namespace AISmart.GAgent.Autogen;


public class ChatAgentProvider : IChatAgentProvider, ITransientDependency
{
    private readonly ChatClient _chatClient;

    private readonly Dictionary<string, MiddlewareAgent<MiddlewareStreamingAgent<OpenAIChatAgent>>> _agents = new();

    public ChatAgentProvider(ChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public MiddlewareAgent<MiddlewareStreamingAgent<OpenAIChatAgent>> GetAgent(string agentName)
    {
        _agents.TryGetValue(agentName, out var agent);
        return agent;
    }

    public void SetAgent(string agentName, string systemMessage, FunctionCallMiddleware middleware)
    {
        var agent = new OpenAIChatAgent(_chatClient, agentName, systemMessage).RegisterMessageConnector()
            .RegisterMessageConnector()
            .RegisterMiddleware(middleware);
        _agents.Add(agentName, agent);
    }
}