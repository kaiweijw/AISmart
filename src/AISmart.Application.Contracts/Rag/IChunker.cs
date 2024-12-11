using System.Collections.Generic;

namespace AISmart.Rag;

public interface IChunker
{
    IEnumerable<string> Chunk(string text);
}