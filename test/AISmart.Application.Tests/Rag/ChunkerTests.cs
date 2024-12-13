using System.Collections.Generic;
using System.Threading.Tasks;
using AISmart.Rag.Agent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;

namespace AISmart.Rag;


public class ChunkerTests : AISmartApplicationTestBase
{
    
    [Fact]
    public async Task TestBaseChunk()
    {
        var configuration = ServiceProvider.GetRequiredService<IConfiguration>();
        var apikey = configuration["Rag:APIKey"];
        var embeddingProvider = new OpenAIEmbeddingProvider(apikey);
        
        string textContent = "Test chunk content.";
        
        float[] results = await embeddingProvider.GetEmbeddingAsync(textContent);

        var s = 1;
    }
}