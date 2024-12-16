using System.Threading;
using System.Threading.Tasks;
using AElf.Indexing.Elasticsearch;
using AISmart.Application.Grains.Dto;
using MediatR;
using Nest;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application;

public class QueryEventCommandHandler : IRequestHandler<QueryEventCommand, CreateTransactionEventIndex>
{
    private readonly INESTRepository<CreateTransactionEventIndex, string> _createTransactionEventIndexRepository;
    private readonly IObjectMapper _objectMapper;
    private readonly IElasticClient _elasticClient;

    public QueryEventCommandHandler(
        IObjectMapper objectMapper,
        INESTRepository<CreateTransactionEventIndex, string> createTransactionEventIndexRepository,        
        IElasticClient elasticClient

    )
    {
        _createTransactionEventIndexRepository = createTransactionEventIndexRepository;
        _objectMapper = objectMapper;
        _elasticClient = elasticClient;

    }
    
    public async Task<CreateTransactionEventIndex> Handle(QueryEventCommand request, CancellationToken cancellationToken)
    {
        return await _createTransactionEventIndexRepository.GetAsync();
    }

}