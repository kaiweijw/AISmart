using AElf.Indexing.Elasticsearch;
using AISmart.Application.Grains.Command;
//using AISmart.Application.Grains.Dto;
using Elasticsearch.Net;
using Nest;
using Volo.Abp.ObjectMapping;
using Volo.Abp.EventBus;

namespace AISmart.Application.Grains.CommandHandler;

public class CreateEventCommandHandler : ILocalEventHandler<CreateEventComamand>
{
   // private readonly INESTRepository<CreateTransactionEventIndex, string> _createTransactionEventIndexRepository;
    private readonly IObjectMapper _objectMapper;
    private readonly IElasticClient _elasticClient;

    public CreateEventCommandHandler(
        IObjectMapper objectMapper,
       // INESTRepository<CreateTransactionEventIndex, string> createTransactionEventIndexRepository,        
        IElasticClient elasticClient

    )
    {
      //  _createTransactionEventIndexRepository = createTransactionEventIndexRepository;
        _objectMapper = objectMapper;
        _elasticClient = elasticClient;

    }
    public async Task HandleEventAsync(CreateEventComamand eventData)
    {
        //var createTransactionEventIndex = _objectMapper.Map<CreateEventComamand, CreateTransactionEventIndex>(eventData);
       // await _createTransactionEventIndexRepository.AddOrUpdateAsync(createTransactionEventIndex);
    }
}