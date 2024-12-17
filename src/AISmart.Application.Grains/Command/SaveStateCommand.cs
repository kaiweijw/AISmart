using System;
using MediatR;

namespace AISmart.Application.Grains.Command;

public class SaveStateCommand : IRequest<int>
{
    public string Id { get; set; }
    public BaseState State { get; set; }

}