using System.Collections.Generic;
using System.Threading.Tasks;
using AISmart.Rag.Agent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;

namespace AISmart.Rag;


public class RagProviderTest : AISmartApplicationTestBase
{
    private readonly string _qdrantUrl = "http://localhost:6333"; 
    private readonly string _collectionName = "test_collection"; 
    
    [Fact]
    public async Task StoreBatchAsync_Test()
    {
        var configuration = ServiceProvider.GetRequiredService<IConfiguration>();
        var apiKey = configuration["Rag:APIKey"];
        var endpoint = configuration["AzureService:endpoint"];
        var azureKey = configuration["AzureService:apiKey"];
        
        var chunker = new MicrosoftChunker(endpoint, azureKey); 
        var embeddingProvider = new OpenAIEmbeddingProvider(apiKey); 
        var vectorDatabase = new QdrantVectorDatabase(_qdrantUrl, _collectionName);

        var ragProvider = new RagProvider(chunker, embeddingProvider, vectorDatabase);

        var texts = new List<string>
        {
            "RAG功能强大。",
            "在欧冠第6轮比赛中",
            "RAG包括chunk, embedding以及vectordb。",
            "姆巴佩破门",
            "RAG通过引入外部文档来缓解LLM的固有限制",
            "帮助皇马客场3-2击败亚特兰大"
        };
        
        await ragProvider.BatchStoreTextsAsync(texts);

        var keyword = "RAG";
        var queryEmbedding = await embeddingProvider.GetEmbeddingAsync(keyword+"是什么？");
        var retrievedChunks = await vectorDatabase.RetrieveAsync(queryEmbedding,3);
        foreach (var chunk in retrievedChunks)
        {
            Assert.Contains(keyword, chunk);
        }
    }
    
}