using AISmart.Agents;
using MediatR;

namespace AISmart.CQRS.Dto;

public class SendEventCommand : IRequest<int>
{
    public EventBase Event { get; set; }
}
