using System;
using System.Collections.Generic;

namespace AISmart.Rag;

public class SimpleChunker : IChunker
{
    public IEnumerable<string> Chunk(string text)
    {
        return text.Split(new[] { ".", "!", "?" }, StringSplitOptions.RemoveEmptyEntries);
    }
}