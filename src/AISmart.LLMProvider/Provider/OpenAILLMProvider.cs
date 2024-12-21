using AISmart.AIProvider;
using AISmart.LLMProvider.Options;
using AutoGen.Core;
using AutoGen.OpenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace AISmart.LLMProvider.Provider;

public class OpenAILLMProvider : ILLMProvider<IMessage>, ITransientDependency
{
    private readonly ILogger<OpenAIChatAgent> _logger;
    private readonly OpenAIOptions _openAiOptions;

    public OpenAILLMProvider(IOptions<OpenAIOptions> options, ILogger<OpenAIChatAgent> logger)
    {
        _logger = logger;
        _openAiOptions = options.Value;
    }

    public Task<IMessage> SendAsync(string message)
    {
        throw new NotImplementedException();
    }

    public Task<IMessage> SendAsync(string message, List<IMessage> history)
    {
        throw new NotImplementedException();
    }

    public async Task<IMessage> SendAsync(string message, List<IMessage> chatHistory, string? description)
    { 
        // IMessage response = await middlewareAgent.SendAsync(message, chatHistory);
        IMessage response = null;
        return response;   
    }

}