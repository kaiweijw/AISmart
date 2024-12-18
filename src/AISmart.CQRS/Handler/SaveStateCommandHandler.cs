using System;
using System.Threading;
using System.Threading.Tasks;
using AISmart.CQRS.Dto;
using MediatR;
using Newtonsoft.Json;
using Volo.Abp.DependencyInjection;

namespace AISmart.CQRS.Handler;

public class SaveStateCommandHandler : IRequestHandler<SaveStateCommand, int>,ISingletonDependency
{
    private readonly IIndexingService  _indexingService ;

    public SaveStateCommandHandler(
        IIndexingService indexingService
    )
    {
        _indexingService = indexingService;
    }

    public async Task<int> Handle(SaveStateCommand request, CancellationToken cancellationToken)
    {
        _indexingService.CheckExistOrCreateIndex(request.State.GetType().Name);
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
        await _indexingService.SaveOrUpdateIndexAsync(request.State.GetType().Name, index);
    }
}