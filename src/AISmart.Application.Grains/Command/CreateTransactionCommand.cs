using System;
using MediatR;

namespace AISmart.Application;

public class CreateTransactionCommand : IRequest<int>
{
    public Guid Id { get; set; }
    public string EventName { get; set; }
    public string EventInfo { get; set; }
    
    public long CreateTime{ get; set; }

}