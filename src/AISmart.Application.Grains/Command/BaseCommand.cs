using MediatR;

namespace AISmart.Application.Grains.Command;

public class BaseCommand<TState> : IRequest<int>
{
    public TState State { get; set; }
}