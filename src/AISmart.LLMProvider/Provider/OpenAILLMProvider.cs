using System.Text;
using AISmart.LLMProvider.Options;
using AutoGen.Core;
using AutoGen.OpenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

    public async Task<IMessage?> SendAsync(string message)
    {
        return await SendAsync(message, null, null);
    }

    public async Task<IMessage?> SendAsync(string message, List<IMessage>? history)
    {
        return await SendAsync(message, history, null);
    }

    public async Task<IMessage?> SendAsync(string message, List<IMessage>? chatHistory, string? description)
    { 
        // Your OpenAI API key
        string apiKey = _openAiOptions.ApiKey;

        // API URL for ChatGPT (GPT-3.5 or GPT-4)
        string apiUrl = _openAiOptions.ApiUrl;

        // Request payload
        var requestData = new
        {
            model = _openAiOptions.Model, // GPT model
            messages = new[]
            {
                new { role = "user", content = message }
            },
            max_tokens = 100,      // Max tokens in response
            temperature = 0.7      // Creativity level
        };

        // Serialize request data to JSON
        var requestJson = JsonConvert.SerializeObject(requestData);

        // Set up HttpClient
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = await client.PostAsync(apiUrl, requestContent);
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
                return Task.FromResult(jsonResponse.choices[0].message.content);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("OpenAILLMProvider SendAsync Exception: " + ex.Message);
            }
        }

        return null;
    }

}