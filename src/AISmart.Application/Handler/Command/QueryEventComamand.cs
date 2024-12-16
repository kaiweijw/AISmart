using AISmart.Application.Grains.Dto;
using MediatR;

namespace AISmart.Application;

public class QueryEventCommand :IRequest<CreateTransactionEventIndex>
{
    public string EventName { get; set; }
}