using System.Threading;
using System.Threading.Tasks;
using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.CommandHandler;
using AISmart.Application.Grains.Dto;
using AISmart.Cqrs.Command;
using MediatR;
using Nest;
using Newtonsoft.Json;

namespace AISmart.Cqrs.CommandHandler;

public class QueryStateCommandHandler : IRequestHandler<SaveStateCommand, int>
{
    private readonly IElasticClient _elasticClient;

    public QueryStateCommandHandler(
        IElasticClient elasticClient
    )
    {
        _elasticClient = elasticClient;
    }

    public async Task<int> Handle(SaveStateCommand request, CancellationToken cancellationToken)
    {
        
        return await Task.FromResult(1); 
    }
}
