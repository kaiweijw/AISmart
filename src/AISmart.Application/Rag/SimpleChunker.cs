using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AISmart.Rag;

public class SimpleChunker : IChunker
{
    public async Task<List<string>> Chunk(string text, int maxChunkSize)
    {
        
        var chunks = new List<string>();
        int length = text.Length;

        for (int i = 0; i < length; i += maxChunkSize)
        {
            int actualChunkSize = maxChunkSize;
            if (i + maxChunkSize < length && char.IsLetterOrDigit(text[i + maxChunkSize]))
            {
                while (i + actualChunkSize < length && char.IsLetterOrDigit(text[i + actualChunkSize]))
                {
                    actualChunkSize++;
                }
            }

            chunks.Add(text.Substring(i, Math.Min(actualChunkSize, length - i)));
        }
        return chunks;
    }
}