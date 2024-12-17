using AISmart.Cqrs.Command;
using MediatR;

namespace AISmart.CQRS.Dto;

public class SaveStateCommand : IRequest<int>
{
    public string Id { get; set; }
    public BaseState State { get; set; }
}
