using System.Threading;
using System.Threading.Tasks;
using AISmart.Application.Grains.Dto;
using AISmart.Cqrs.Command;
using MediatR;
using Nest;

namespace AISmart.Cqrs.CommandHandler;

public class SaveStateCommandHandler : IRequestHandler<SaveStateCommand, int>
{
    private readonly IElasticClient _elasticClient;
    private const string _indxeName = "newstateindex";

    public SaveStateCommandHandler(
         IElasticClient elasticClient
    )
    {
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
