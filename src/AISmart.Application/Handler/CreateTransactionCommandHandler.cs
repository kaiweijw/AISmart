using System.Threading;
using System.Threading.Tasks;
using AISmart.Application.Grains.Dto;
using MediatR;
using Nest;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, int>
{
    private readonly IObjectMapper _objectMapper;
    private readonly IElasticClient _elasticClient;
    public CreateTransactionCommandHandler(
        IObjectMapper objectMapper
        , IElasticClient elasticClient
    )
    {
        _objectMapper = objectMapper;
        _elasticClient = elasticClient;
        CreateIndex(_elasticClient);
    }

    public async Task<int> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
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
        var indexData = new EventIndex
        {
            EventType = "type1"
        };
        var response = await _elasticClient.IndexAsync(indexData, i => i
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