using System;
using System.Threading;
using System.Threading.Tasks;
using AISmart.CQRS.Dto;
using MediatR;
using Nest;
using Newtonsoft.Json;

namespace AISmart.CQRS.Handler;

public class SaveStateCommandHandler : IRequestHandler<SaveStateCommand, int>
{
    private readonly IElasticClient _elasticClient;
    private readonly IElasticIndexService _elasticIndexService;

    public SaveStateCommandHandler(
        IElasticClient elasticClient,
        IElasticIndexService elasticIndexService
    )
    {
        _elasticClient = elasticClient;
        _elasticIndexService = elasticIndexService;
    }

    public async Task<int> Handle(SaveStateCommand request, CancellationToken cancellationToken)
    {
        await _elasticIndexService.CheckExistOrCreateIndex(request.State.GetType().Name);
        await SaveIndexAsync(request);
        return await Task.FromResult(1); 
    }

    private async Task SaveIndexAsync(SaveStateCommand request)
    {
        var index = new BaseStateIndex
        {
            Id = request.Id,
            Ctime = DateTime.Now,
            State = JsonConvert.SerializeObject(request.State)
        };
        await _elasticIndexService.SaveOrUpdateIndexAsync(request.State.GetType().Name, index);
    }
}