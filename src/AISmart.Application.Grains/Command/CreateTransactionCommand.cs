using AISmart.Agents;
using MediatR;

namespace AISmart.Application.Grains.Command;

public class CreateTransactionCommand : IRequest<int>
{
    public Guid Id { get; set; }
    public string EventName { get; set; }
    public string EventInfo { get; set; }

}