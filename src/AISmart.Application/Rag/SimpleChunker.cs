using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AISmart.Rag;

public class SimpleChunker : IChunker
{
    public async Task<List<string>> Chunk(string text, int chunkSize)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) 
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) 
            .Build();

        var endpoint = config["AppSettings:endpoint"];
        var apiKey = config["AppSettings:apiKey"];

        var helper = new AzureTextAnalyticsHelper(endpoint, apiKey);
        return await helper.SmartChunkTextAsync(text, chunkSize);
    }
    
}