using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using OpenAI.Chat;
using Volo.Abp.DependencyInjection;

namespace AISmart.GAgent.Autogen;

public interface IChatAgentProvider
{
    MiddlewareAgent<MiddlewareStreamingAgent<OpenAIChatAgent>> GetAgent(string agentName);

    void SetAgent(string agentName, string systemMessage, FunctionCallMiddleware middleware);
}
