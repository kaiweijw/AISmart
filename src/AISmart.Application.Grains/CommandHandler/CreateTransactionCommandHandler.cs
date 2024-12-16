using AISmart.Application.Grains.Command;
using AISmart.Application.Grains.Dto;
using MediatR;
using Nest;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application.Grains.CommandHandler;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, int>
{
    private readonly IObjectMapper _objectMapper;
    private readonly IElasticClient _elasticClient;
    public CreateTransactionCommandHandler(
        IObjectMapper objectMapper
    )
    {
        _objectMapper = objectMapper;
    }

    public async Task<int> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
            .DefaultIndex("createtransactioneventindex")
            .DefaultMappingFor<CreateTransactionEventIndex>(m => m.IndexName("createtransactioneventindex")); 
        var client = new ElasticClient(settings);

        var createTransactionEventIndex = _objectMapper.Map<CreateTransactionCommand, CreateTransactionEventIndex>(request);
        
        client.IndexDocument(createTransactionEventIndex);
        return await Task.FromResult(1); 

    }
}