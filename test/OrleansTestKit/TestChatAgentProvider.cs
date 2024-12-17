using AISmart.GAgent.Autogen;
using Moq;
using AutoGen.Core;
using AutoGen.OpenAI;
using Volo.Abp.DependencyInjection;

public class TestChatAgentProvider : IChatAgentProvider, ITransientDependency
{
    public MiddlewareAgent<MiddlewareStreamingAgent<OpenAIChatAgent>> GetAgent(string agentName)
    {
        var mock = new Mock<MiddlewareAgent<MiddlewareStreamingAgent<OpenAIChatAgent>>>();
        return mock.Object;
    }

    public void SetAgent(string agentName, string systemMessage, FunctionCallMiddleware middleware)
    {
    }
}