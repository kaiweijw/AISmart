using System.Collections.Generic;
using System.Threading.Tasks;
using AISmart.Rag.Agent;
using Xunit;
using Moq;

namespace AISmart.Rag;


public class RagProviderTest : AISmartApplicationTestBase
{
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
}