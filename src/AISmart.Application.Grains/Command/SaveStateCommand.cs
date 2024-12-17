using System;
using AISmart.Cqrs.Command;
using MediatR;

namespace AISmart.Application.Grains.Command;

public class SaveState1Command : IRequest<int>
{
    public string Id { get; set; }
    public BaseState State { get; set; }

}