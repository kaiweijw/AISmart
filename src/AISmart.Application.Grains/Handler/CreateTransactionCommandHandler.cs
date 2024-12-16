using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Dto;
using MediatR;
using Nest;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application.Grains.Handler;

public class CreateTransaction1CommandHandler : IRequestHandler<DeveloperAgentCommand, int>
{
    private readonly IObjectMapper _objectMapper;
    private readonly IElasticClient _elasticClient;
    public CreateTransaction1CommandHandler(
        IObjectMapper objectMapper
        , IElasticClient elasticClient
    )
    {
        _objectMapper = objectMapper;
        _elasticClient = elasticClient;
        CreateIndex(_elasticClient);
    }

    public async Task<int> Handle(DeveloperAgentCommand request, CancellationToken cancellationToken)
    {
        // var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
        //     .DefaultIndex("createtransactioneventindex")
        //     .DefaultMappingFor<CreateTransactionEventIndex>(m => m.IndexName("createtransactioneventindex")); 
        // var client = new ElasticClient(settings);
        //
        // var createTransactionEventIndex = _objectMapper.Map<CreateTransactionCommand, CreateTransactionEventIndex>(request);
        //
        // client.IndexDocument(createTransactionEventIndex);
        var documentId = request.Id.ToString();
        var state = request.State;
        var indexData = new EventIndex
        {
            EventType = "type1"
        };
        var response = await _elasticClient.IndexAsync(state, i => i
            .Index("eventindex")
            .Id(documentId)
        );
        return await Task.FromResult(1); 
    }
    
    public static void CreateIndex(IElasticClient elasticClient)
    {
        var createIndex1Response = elasticClient.Indices.Create("eventindex", c => c
            .Map<EventIndex>(m => m
                .AutoMap()
            )
        );
    }


}