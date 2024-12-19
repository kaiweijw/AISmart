using System;
using System.Threading;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.Group;
using AISmart.CQRS.Dto;
using AISmart.Sender;
using MediatR;
using Orleans;
using Volo.Abp.DependencyInjection;

namespace AISmart.CQRS.Handler;

public class SendEventCommandHandler : IRequestHandler<SendEventCommand, int>,ISingletonDependency
{
    private readonly IClusterClient _clusterClient;

    public SendEventCommandHandler(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public async Task<int> Handle(SendEventCommand request, CancellationToken cancellationToken)
    {
        var publishingAgent = _clusterClient.GetGrain<IPublishingGAgent>(Guid.NewGuid());
        var groupAgent = _clusterClient.GetGrain<IStateGAgent<GroupAgentState>>(Guid.NewGuid());

        await publishingAgent.PublishTo(groupAgent);
        await publishingAgent.PublishEventAsync(request.Event);
        return await Task.FromResult(1); 
    }
    
}