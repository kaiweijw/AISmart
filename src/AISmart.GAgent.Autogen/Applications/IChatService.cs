using AutoGen.Core;
using Volo.Abp.DependencyInjection;

namespace AISmart.GAgent.Autogen.Applications;

public interface IChatService
{
    Task<IMessage> SendAsync(string agentName, string message, IEnumerable<IMessage>? chatHistory);
}

public class ChatService : IChatService, ITransientDependency
{
    private readonly IChatAgentProvider _chatAgentProvider;

    public ChatService(IChatAgentProvider chatAgentProvider)
    {
        _chatAgentProvider = chatAgentProvider;
    }

    public async Task<IMessage> SendAsync(string agentName, string message, IEnumerable<IMessage>? chatHistory)
    {
        var agent = _chatAgentProvider.GetAgent(agentName);
        return await agent.SendAsync(message, chatHistory);
    }
}