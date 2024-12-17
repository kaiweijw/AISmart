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

    public void CheckExistOrCreateIndex(string typeName)
    {
        var indexName = typeName.ToLower() + "index";
        var indexExistsResponse = _elasticClient.Indices.Exists(indexName);
        if (indexExistsResponse.Exists)
        {
            return;
        }
        var createIndexResponse = _elasticClient.Indices.Create(indexName, c => c
            .Map<BaseStateIndex>(m => m.AutoMap())
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

    public async Task SaveOrUpdateIndexAsync(string typeName, BaseStateIndex baseStateIndex)
    {
        var indexName = typeName.ToLower() + "index";
        await _elasticClient.IndexAsync(baseStateIndex, i => i
            .Index(indexName)
            .Id(baseStateIndex.Id)
        );
    }
}