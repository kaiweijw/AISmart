using System.Threading.Tasks;

namespace AISmart.Rag;

public class RagProvider
{
    private readonly IChunker _chunker;
    private readonly IEmbeddingProvider _embeddingProvider;
    private readonly IVectorDatabase _vectorDatabase;

    public RagProvider(IChunker chunker, IEmbeddingProvider embeddingProvider, IVectorDatabase vectorDatabase)
    {
        _chunker = chunker;
        _embeddingProvider = embeddingProvider;
        _vectorDatabase = vectorDatabase;
    }

    public async Task StoreTextAsync(string text)
    {
        var chunks = _chunker.Chunk(text);
        foreach (var chunk in chunks)
        {
            var embedding = await _embeddingProvider.GetEmbeddingAsync(chunk);
            await _vectorDatabase.StoreAsync(chunk, embedding);
        }
    }

    public async Task<string> RetrieveAnswerAsync(string query)
    {
        var queryEmbedding = await _embeddingProvider.GetEmbeddingAsync(query);
        var relevantChunks = await _vectorDatabase.RetrieveAsync(queryEmbedding, 3);
        return string.Join(" ", relevantChunks);
    }
}