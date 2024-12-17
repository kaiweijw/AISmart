using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.CommandHandler;
using AISmart.Application.Grains.Dto;
using MediatR;
using Nest;
using Newtonsoft.Json;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application.Grains.Command;

public class SaveStateCommandHandler : IRequestHandler<SaveStateCommand, int>
{
    private readonly IObjectMapper _objectMapper;
    private readonly IElasticClient _elasticClient;
    private const string _indxeName = "stateindex";

    public SaveStateCommandHandler(
        IObjectMapper objectMapper
        , IElasticClient elasticClient
    )
    {
        _objectMapper = objectMapper;
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
    
    public static async Task CreateIndexByTypeAsync(IElasticClient elasticClient, BaseState state)
    {
        var indexService = new ElasticIndexService(elasticClient);
        var stateType = state.GetType();
        await indexService.CreateIndexFromTypeAsync(stateType.Name);
    }
    
    public async Task SaveIndexAsync(SaveStateCommand request)
    {
        var documentId = request.Id.ToString();
        var typeName = request.State.GetType().Name;
        var indexName = request.State.GetType().Name.ToLower() + "index";
        var state = request.State;
        switch (typeName)
        {
            case nameof(DeveloperAgentState):
                var dto = (DeveloperAgentState)state;
                var index = new DeveloperAgentStateIndex()
                {
                    Id = documentId,
                    Content = JsonConvert.SerializeObject(dto.Content)
                };
                await _elasticClient.IndexAsync(index, i => i
                    .Index(indexName)
                    .Id(documentId)
                );
                break;
            default:
                break;
        }
    }


}