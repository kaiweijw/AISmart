using System.Threading.Tasks;
using AISmart.Cqrs.Command;
using MediatR;
using Volo.Abp.DependencyInjection;

namespace AISmart.Cqrs;

public class CqrsProvider : ICqrsProvider, ISingletonDependency
{
    private readonly IMediator _mediator;
    public CqrsProvider(IMediator mediator)
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