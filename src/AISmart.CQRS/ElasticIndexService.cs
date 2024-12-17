using System.Threading.Tasks;
using AISmart.CQRS.Dto;
using Microsoft.Extensions.Logging;
using Nest;

namespace AISmart.CQRS;

public class ElasticIndexService : IElasticIndexService
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger Logger;

    public ElasticIndexService(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task CreateIndexFromEntity(string typeName)
    {
        var indexName = typeName.ToLower() + "index";
        var createIndexResponse = _elasticClient.Indices.Create(indexName, c => c
            .Map<BaseStateIndex>(m=>m.AutoMap())
        );
        if (!createIndexResponse.IsValid)
        {
            Logger.LogError("Error creating index {error}", createIndexResponse.ServerError?.Error);
        }
        else
        {
            Logger.LogError("Index created successfully. {indexName}", indexName);
        }
    }
}