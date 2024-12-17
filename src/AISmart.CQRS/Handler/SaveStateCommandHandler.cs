using System.Threading;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.CQRS.Dto;
using MediatR;
using Nest;

namespace AISmart.CQRS.Handler;

public class SaveStateCommandHandler : IRequestHandler<SaveStateCommand, int>
{
    private readonly IElasticClient _elasticClient;
    private readonly IElasticIndexService _elasticIndexService;

    public SaveStateCommandHandler(
        IElasticClient elasticClient,
        IElasticIndexService elasticIndexService
    )
    {
        _elasticClient = elasticClient;
        _elasticIndexService = elasticIndexService;
    }

    public async Task<int> Handle(SaveStateCommand request, CancellationToken cancellationToken)
    {
        _elasticIndexService.CreateIndexFromEntity(request.State.GetType().Name);
       
        await SaveIndexAsync(request);
        return await Task.FromResult(1); 
    }

    private async Task SaveIndexAsync(SaveStateCommand request)
    {
        var documentId = request.Id.ToString();
        var typeName = request.State.GetType().Name;
        var indexName = request.State.GetType().Name.ToLower() + "index";
        var state = request.State;
        // switch (typeName)
        // {
        //     case nameof(DeveloperAgentState):
        //         var dto = (DeveloperAgentState)state;
        //         var index = new DeveloperAgentStateIndex()
        //         {
        //             Id = documentId,
        //             Content = JsonConvert.SerializeObject(dto.Content)
        //         };
        //         await _elasticClient.IndexAsync(index, i => i
        //             .Index(indexName)
        //             .Id(documentId)
        //         );
        //         break;
        // }
    }
}