using System.Threading;
using System.Threading.Tasks;
// using AElf.Indexing.Elasticsearch.Services;
using AISmart.Cqrs.Command;
using AISmart.CQRS.Dto;
using MediatR;
using Nest;
using Newtonsoft.Json;

namespace AISmart.CQRS.Handler;

public class SaveStateCommandHandler : IRequestHandler<SaveStateCommand, int>
{
    private readonly IElasticClient _elasticClient;

    public SaveStateCommandHandler(
        IElasticClient elasticClient
    )
    {
        _elasticClient = elasticClient;
    }

    public async Task<int> Handle(SaveStateCommand request, CancellationToken cancellationToken)
    {
        //await CreateIndex(_elasticClient, request.State);
        await CreateIndexByTypeAsync(_elasticClient, request.State);
       
        await SaveIndexAsync(request);
        return await Task.FromResult(1); 
    }
    
    public static async Task CreateIndexAsync(IElasticClient elasticClient, BaseState state)
    {
        var indexService = new ElasticIndexService(elasticClient);
        var stateType = state.GetType();
        
        var method = typeof(ElasticIndexService).GetMethod("CreateIndexFromEntityAsync");
        var genericMethod = method.MakeGenericMethod(stateType);
        
        await (Task)genericMethod.Invoke(indexService, null);
    }

    private static async Task CreateIndexByTypeAsync(IElasticClient elasticClient, BaseState state)
    {
        var indexService = new ElasticIndexService(elasticClient);
        var stateType = state.GetType();
        await indexService.CreateIndexFromTypeAsync(stateType.Name);
    }
    
    private async Task SaveIndexAsync(SaveStateCommand request)
    {
        var documentId = request.Id.ToString();
        var typeName = request.State.GetType().Name;
        var indexName = request.State.GetType().Name.ToLower() + "index";
        var state = request.State;
        // switch (typeName)
        // {
        //     case nameof(DeveloperAgentState):
        //         var dto = (DeveloperAgentState)state;
        //         var index = new DeveloperAgentStateIndex()
        //         {
        //             Id = documentId,
        //             Content = JsonConvert.SerializeObject(dto.Content)
        //         };
        //         await _elasticClient.IndexAsync(index, i => i
        //             .Index(indexName)
        //             .Id(documentId)
        //         );
        //         break;
        // }
    }
}