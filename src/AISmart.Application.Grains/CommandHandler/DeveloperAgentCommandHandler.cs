using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Dto;
using MediatR;
using Nest;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application.Grains.Handler;

public class DeveloperAgentCommandHandler : IRequestHandler<DeveloperAgentCommand, int>
{
    private readonly IObjectMapper _objectMapper;
    private readonly IElasticClient _elasticClient;
    private const string _indxeName = "stateindex";
    public DeveloperAgentCommandHandler(
        IObjectMapper objectMapper
        , IElasticClient elasticClient
    )
    {
        _objectMapper = objectMapper;
        _elasticClient = elasticClient;
        CreateIndex(_elasticClient);
    }

    public async Task<int> Handle(DeveloperAgentCommand request, CancellationToken cancellationToken)
    {
        var documentId = request.Id.ToString();
        var state = request.State;
        var response = await _elasticClient.IndexAsync(state, i => i
            .Index(_indxeName)
            .Id(documentId)
        );
        return await Task.FromResult(1); 
    }
    
    public static void CreateIndex(IElasticClient elasticClient)
    {
        var createIndex1Response = elasticClient.Indices.Create(_indxeName, c => c
            .Map<EventIndex>(m => m
                .AutoMap()
            )
        );
    }


}