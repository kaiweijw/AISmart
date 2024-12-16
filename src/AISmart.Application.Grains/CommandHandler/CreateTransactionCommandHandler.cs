using AElf.Indexing.Elasticsearch;
using AISmart.Application.Grains.Command;
using AISmart.Application.Grains.Dto;
using Elasticsearch.Net;
using MediatR;
using Nest;
using Volo.Abp.ObjectMapping;
using Volo.Abp.EventBus;

namespace AISmart.Application.Grains.CommandHandler;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, int>
{
    //private readonly INESTRepository<CreateTransactionEventIndex, string> _createTransactionEventIndexRepository;
    private readonly IObjectMapper _objectMapper;

    public CreateTransactionCommandHandler(
        IObjectMapper objectMapper
        //INESTRepository<CreateTransactionEventIndex, string> createTransactionEventIndexRepository
    )
    {
       // _createTransactionEventIndexRepository = createTransactionEventIndexRepository;
        _objectMapper = objectMapper;

    }
    

    /*public async Task Handle(CammandBase request, CancellationToken cancellationToken)
    {
        switch (request)
        {
            /*case SpecificCommandA commandA:
                // Handle SpecificCommandA
                Console.WriteLine($"Handling SpecificCommandA with PropertyA: {commandA.PropertyA}");
                break;
            case SpecificCommandB commandB:
                // Handle SpecificCommandB
                Console.WriteLine($"Handling SpecificCommandB with PropertyB: {commandB.PropertyB}");
                break;
            default:
                throw new NotSupportedException($"Command of type {command.GetType().Name} is not supported.");#1#
        }
        var createTransactionEventIndex = _objectMapper.Map<CammandBase, CreateTransactionEventIndex>(request);
        
        await _createTransactionEventIndexRepository.AddOrUpdateAsync(createTransactionEventIndex);
        //return await Task.FromResult(1); 
    }*/

    public async Task<int> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var createTransactionEventIndex = _objectMapper.Map<CreateTransactionCommand, CreateTransactionEventIndex>(request);
        
       // await _createTransactionEventIndexRepository.AddOrUpdateAsync(createTransactionEventIndex);
        return await Task.FromResult(1); 

    }
}