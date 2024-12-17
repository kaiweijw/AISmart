using System.Threading;
using System.Threading.Tasks;
using AISmart.Application.Grains.Dto;
using MediatR;
using Nest;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application.Grains.Command;

public class SaveStateCommandHandler : IRequestHandler<SaveStateCommand, int>
{
    private readonly IObjectMapper _objectMapper;
    private readonly IElasticClient _elasticClient;
    private const string _indxeName = "stateindex";

    public SaveStateCommandHandler(
        IObjectMapper objectMapper
        , IElasticClient elasticClient
    )
    {
        _objectMapper = objectMapper;
        _elasticClient = elasticClient;
        CreateIndex(_elasticClient);
    }

    public async Task<int> Handle(SaveStateCommand request, CancellationToken cancellationToken)
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