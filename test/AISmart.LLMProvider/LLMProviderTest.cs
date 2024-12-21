using AISmart.LLMProvider.Options;
using AISmart.LLMProvider.Provider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace AISmart.LLMProvider;


public class LLMProviderTest : AISmartTestBase<AISmartAutoGenTestModule>
{
    [Fact]
    public async Task AzureOpenAI_Test()
    {
        var config = GetRequiredService<IOptions<AzureAIOptions>>();
        var logger = GetRequiredService<ILogger<AzureOpenAILLMProvider>>();
        var azureOpenAillmProvider = new AzureOpenAILLMProvider(config, logger);

        var question = $"what is aelf?";


        var answer = await azureOpenAillmProvider.SendAsync(question);
        Assert.Contains("aelf", answer.Content);
    }
    
}