using AISmart.Agents;
using AISmart.Application.Grains.Dto;
using MediatR;

namespace AISmart.Application.Grains.Command;

public class QueryEventCommand :IRequest<CreateTransactionEventIndex>
{
    public string EventName { get; set; }
}