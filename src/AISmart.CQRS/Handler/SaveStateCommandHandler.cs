using System;
using System.Threading;
using System.Threading.Tasks;
using AISmart.CQRS.Dto;
using AISmart.CQRS.Service;
using MediatR;
using Newtonsoft.Json;

namespace AISmart.CQRS.Handler;

public class SaveStateCommandHandler : IRequestHandler<SaveStateCommand>
{
    //private readonly IIndexingService  _indexingService ;
    private readonly KafkaProducerService _kafkaProducerService;

    public SaveStateCommandHandler(
       // IIndexingService indexingService
       KafkaProducerService kafkaProducerService
    )
    {
       // _indexingService = indexingService;
       _kafkaProducerService = kafkaProducerService;
    }

    public async Task<Unit> Handle(SaveStateCommand request, CancellationToken cancellationToken)
    {
        /*_indexingService.CheckExistOrCreateIndex(request.State.GetType().Name);
        await SaveIndexAsync(request);*/
        await _kafkaProducerService.SendAsync(request);
        return Unit.Value;
    }

    /*private async Task SaveIndexAsync(SaveStateCommand request)
    {
        var index = new BaseStateIndex
        {
            Id = request.Id,
            Ctime = DateTime.Now,
            State = JsonConvert.SerializeObject(request.State)
        };
        await _indexingService.SaveOrUpdateIndexAsync(request.State.GetType().Name, index);
    }*/
}