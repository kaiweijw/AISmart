using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AISmart.Rag;

public class OpenAIEmbeddingProvider : IEmbeddingProvider
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public OpenAIEmbeddingProvider()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        IConfiguration config = builder.Build();

        _apiKey = config["Rag:APIKey"];
        _httpClient = new HttpClient();
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var requestBody = new { input = text, model = "text-embedding-ada-002" };
        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/embeddings", content);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<EmbedResponse>(responseString).Data[0].Embedding;
    }

    private class EmbedResponse
    {
        public List<EmbedData> Data { get; set; }
    }

    private class EmbedData
    {
        public float[] Embedding { get; set; }
    }
}