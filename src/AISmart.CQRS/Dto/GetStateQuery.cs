using AISmart.Agents;
using MediatR;

namespace AISmart.CQRS.Dto;

public class GetStateQuery : IRequest<BaseState>
{
    public string Id { get; set; }
    public string Index { get; set; }
}