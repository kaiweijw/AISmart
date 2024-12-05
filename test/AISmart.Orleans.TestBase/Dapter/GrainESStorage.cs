using System;
using System.Threading.Tasks;
using Nest;
using Newtonsoft.Json;
using Orleans;
using Orleans.Runtime;
using Orleans.Storage;

namespace AISmart.Dapter;

public class GrainESStorage: IGrainStorage
{
    private readonly IElasticClient _elasticClient;
    private static readonly string _indexName = "grain-event";

    public GrainESStorage(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task ReadStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState) 
    {
        var documentId = GetDocumentId(stateName, grainId, grainState.ETag);
        var response = await _elasticClient.GetAsync<GrainState<T>>(documentId, g => g.Index(_indexName));
        
        if (response.Found)
        {
            var storedState = response.Source;
            grainState.State = storedState.State;
            grainState.ETag = storedState.ETag;
            grainState.RecordExists = true;
        }
        else
        {
            grainState.State = Activator.CreateInstance<T>();
            grainState.ETag = null;
            grainState.RecordExists = false;
        }
    }

    public async Task WriteStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        var documentId = GetDocumentId(stateName, grainId, grainState.ETag);
        
        var response = await _elasticClient.IndexAsync(grainState, i => i
                .Index(_indexName)
                .Id(documentId)
        );

        if (!response.IsValid)
        {
            throw new Exception($"Failed to write state for Grain {grainId} of state {stateName}: {response.ServerError?.Error?.Reason}");
        }

    }

    public Task ClearStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        throw new System.NotImplementedException();
    }
    
    
    private string GetDocumentId(string grainType, GrainId grainId, string eTag)
    {
        return $"{grainType}-{grainId.ToString()}-{eTag}";
    }
}