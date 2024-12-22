using AISmart.LLMProvider.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Volo.Abp.DependencyInjection;

namespace AISmart.LLMProvider.Provider;

public class AzureOpenAILLMProvider : ILLMProvider<ChatMessageContent>, ITransientDependency
{
    private readonly ILogger<AzureOpenAILLMProvider> _logger;

    private readonly AzureAIOptions _autogenOptions;

    public AzureOpenAILLMProvider(IOptions<AzureAIOptions> options, ILogger<AzureOpenAILLMProvider> logger)
    {
        _logger = logger;
        _autogenOptions = options.Value;
    }

    public Task<ChatMessageContent?> SendAsync(string message)
    {
        return SendAsync(message, null, null);
    }

    public Task<ChatMessageContent?> SendAsync(string message, List<ChatMessageContent>? history)
    {
        return SendAsync(message, history, null);
    }

    public async Task<ChatMessageContent?> SendAsync(string message, List<ChatMessageContent>? history,
        string? description)
    { 
        _logger.LogInformation("AzureAIProvider SendAsync start, message: {message}", message);
        // Populate values from your OpenAI deployment
        var modelId = _autogenOptions.Model;
        var endpoint = _autogenOptions.Endpoint;
        var apiKey = _autogenOptions.ApiKey;

        // Create a kernel with Azure OpenAI chat completion
        var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

        // Build the kernel
        Kernel kernel = builder.Build();
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        ChatHistory chatHistory = new ChatHistory();
        chatHistory.AddRange(history);
        
        // Get the response from the AI
        var result = await chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            kernel: kernel);
        return result;
    }
}