using System.Threading;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.CQRS.Dto;
using MediatR;
using Nest;

namespace AISmart.CQRS.Handler;

public class GetStateQueryHandler : IRequestHandler<GetStateQuery, BaseState>
{
    private readonly IElasticClient _elasticClient;
    public GetStateQueryHandler(
        IElasticClient elasticClient
    )
    {
        _elasticClient = elasticClient;
    }
    
    public async Task<BaseState> Handle(GetStateQuery request, CancellationToken cancellationToken)
    {
        var response = await _elasticClient.GetAsync<BaseState>(request.Id, g => g.Index(request.Index), cancellationToken);
        return response.Source; 
    }
}