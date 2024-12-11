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
    public async Task BackendAgent_ShouldStoreAndRetrieveIndependently()
    {
        var chunkerMock = new Mock<IChunker>();
        chunkerMock.Setup(m => m.Chunk(It.IsAny<string>())).Returns(new List<string> { "Backend data 1", "Backend data 2" });

        var embeddingMock = new Mock<IEmbeddingProvider>();
        embeddingMock.Setup(m => m.GetEmbeddingAsync(It.IsAny<string>())).ReturnsAsync(new float[] { 0.1f, 0.2f });

        var databaseMock = new Mock<IVectorDatabase>();
        databaseMock.Setup(m => m.RetrieveAsync(It.IsAny<float[]>(), It.IsAny<int>())).ReturnsAsync(new List<string> { "Backend data 2" });

        var ragProvider = new RagProvider(chunkerMock.Object, embeddingMock.Object, databaseMock.Object);
        var backendAgent = new BackendAgent(ragProvider);


        await backendAgent.ProcessTextAsync("Backend data about AISmart");
        var answer = await backendAgent.AskAsync("Some question for AISmart");

        Assert.Equal("Backend data 2", answer);
    }

    [Fact]
    public async Task FrontendAgent_ShouldStoreAndRetrieveIndependently()
    {
        var chunkerMock = new Mock<IChunker>();
        chunkerMock.Setup(m => m.Chunk(It.IsAny<string>())).Returns(new List<string> { "Frontend data 1", "Frontend data 2" });

        var embeddingMock = new Mock<IEmbeddingProvider>();
        embeddingMock.Setup(m => m.GetEmbeddingAsync(It.IsAny<string>())).ReturnsAsync(new float[] { 0.3f, 0.4f });

        var databaseMock = new Mock<IVectorDatabase>();
        databaseMock.Setup(m => m.RetrieveAsync(It.IsAny<float[]>(), It.IsAny<int>())).ReturnsAsync(new List<string> { "Frontend data 1" });

        var ragProvider = new RagProvider(chunkerMock.Object, embeddingMock.Object, databaseMock.Object);
        var frontendAgent = new FrontendAgent(ragProvider);

     
        await frontendAgent.ProcessTextAsync("Frontend data about AISmart");
        var answer = await frontendAgent.AskAsync("Some question about AISmart");


        Assert.Equal("Frontend data 1", answer);
    }
    
    [Fact]
    public async Task StoreBatchAsync_Test()
    {
        var configuration = ServiceProvider.GetRequiredService<IConfiguration>();
        var apiKey = configuration["Rag:APIKey"];
        
        var chunker = new SimpleChunker(); 
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