using MediatR;

namespace AISmart.Application.Grains.Command;

public class CammandBase : IRequest<int>, IRequest<Unit>
{
    
}