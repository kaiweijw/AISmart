using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.CQRS.Dto;
using MediatR;
using Volo.Abp.DependencyInjection;

namespace AISmart.CQRS.Provider;

public class CQRSProvider : ICQRSProvider, ISingletonDependency
{
    private readonly IMediator _mediator;
    public CQRSProvider(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public Task Publish(BaseState state, string id)
    {
        var command = new SaveStateCommand
        {
            Id = id,
            State = state
        };
        return _mediator.Send(command);
    }
}