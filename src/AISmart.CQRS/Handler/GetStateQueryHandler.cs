using System.Threading;
using System.Threading.Tasks;
using AISmart.Cqrs.Command;
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
        // var response = await _elasticClient.IndexAsync(request.Index, i => i
        //     .Index(request.Index)
        //     .Id(request.StateId)
        var response = await _elasticClient.GetAsync<BaseState>(request.Id, g => g.Index(request.Index));
        return response.Source; 
    }
}