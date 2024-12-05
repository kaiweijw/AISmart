using System;
using System.Threading.Tasks;
using AISmart.Domain.Grains.Event;
using Nest;
using Orleans;
using Orleans.Runtime;
using Orleans.Storage;

namespace AISmart.Dapter;

public class GrainESStorage: IGrainStorage
{
    private readonly IElasticClient _elasticClient;
    private static readonly string _EventFlowTemplateStateIndex = "eventflowtemplatestate";
    private static readonly string _AgentTaskStateIndex = "agenttaskstate";

    public GrainESStorage(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
        CreateIndex(_elasticClient);
    }

    public async Task ReadStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState) 
    {
        var documentId = GetDocumentId(stateName, grainId, grainState.ETag);

        GetResponse<GrainState<T>> response = null;
        if (grainState is GrainState<AgentTaskState> agentTaskState)
        {
            response = await _elasticClient.GetAsync<GrainState<T>>(documentId, g => g.Index(_AgentTaskStateIndex));
        }
        
        if (grainState is GrainState<EventFlowTemplateState> eventFlowTemplateState)
        {
            response = await _elasticClient.GetAsync<GrainState<T>>(documentId, g => g.Index(_EventFlowTemplateStateIndex));
        }
        
        if (response != null && response.Found)
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

        IndexResponse response;
        response = null;

        if (grainState is GrainState<AgentTaskState> agentTaskState)
        {
            response = await _elasticClient.IndexAsync(grainState, i => i
                .Index(_AgentTaskStateIndex)
                .Id(documentId)
            );
        }
        
        if (grainState is GrainState<EventFlowTemplateState> eventFlowTemplateState)
        {
            response = await _elasticClient.IndexAsync(grainState, i => i
                .Index(_EventFlowTemplateStateIndex)
                .Id(documentId)
            );
        }
        
        if (response != null && !response.IsValid)
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
    
    public static void CreateIndex(IElasticClient elasticClient)
    {
        var createIndex1Response = elasticClient.Indices.Create(_EventFlowTemplateStateIndex, c => c
            .Map<EventFlowTemplateState>(m => m
                .AutoMap()
            )
        );
        
        var createIndex2Response = elasticClient.Indices.Create(_AgentTaskStateIndex, c => c
            .Map<AgentTaskState>(m => m
                .AutoMap()
            )
        );

        // if (!createIndexResponse.IsValid)
        // {
        //     Console.WriteLine($"Failed to create index: {createIndexResponse.ServerError?.Error?.Reason}");
        //     throw new Exception($"Error creating index: {createIndexResponse.ServerError?.Error?.Reason}");
        // }
    }
}