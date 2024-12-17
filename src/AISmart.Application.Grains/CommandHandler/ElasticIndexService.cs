using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Dto;
using AISmart.Cqrs.Command;
using Microsoft.Extensions.Logging;
using Nest;

namespace AISmart.Application.Grains.CommandHandler;

public class ElasticIndexService
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger Logger;

    public ElasticIndexService(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task CreateIndexFromEntityAsync<T>() where T : BaseState
    {
        var indexName = typeof(T).Name.ToLower() + "index";
        var type = typeof(T);
        var createIndexResponse = await _elasticClient.Indices.CreateAsync(indexName, c => c
            .Map<T>(m => m
                .AutoMap() 
                .Properties(props =>
                {
                    foreach (var prop in typeof(T).GetProperties())
                    {
                        if (prop.PropertyType == typeof(List<string>))
                        {
                            props.Object<object>(t => t.Name(prop.Name)); 
                        }
                        if (prop.PropertyType == typeof(string))
                        {
                            props.Text(t => t.Name(prop.Name));
                        }
                        else if (prop.PropertyType == typeof(DateTime))
                        {
                            props.Date(d => d.Name(prop.Name));
                        }
                        else if (prop.PropertyType == typeof(int))
                        {
                            props.Number(n => n.Name(prop.Name).Type(NumberType.Integer));
                        }
                    }

                    return props;
                })
            )
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
    
    public async Task CreateIndexFromTypeAsync(string typeName)
    {
        var indexName = typeName.ToLower() + "index";

        switch (typeName)
        {
            case nameof(DeveloperAgentState):
                _elasticClient.Indices.Create(indexName, c => c
                    .Map<DeveloperAgentStateIndex>(m => m
                        .AutoMap()
                    )
                );
                break;
        }
    }

}