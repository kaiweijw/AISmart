using MediatR;

namespace AISmart.Application.Grains.Command;

public class BaseCommand<TState> : IRequest<int>
{
    public TState State { get; set; }
}
/*var type = State.GetType();
Mediator = ServiceProvider.GetRequiredService<IMediator>();
var command = new DeveloperAgentCommand
{
    State = State,
    Id = Guid.NewGuid().ToString()
};

Mediator.Send(command);
base.OnStateChanged();   */