using System;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.Group;
using AISmart.Sender;
using AISmart.Services;
using Orleans;
using Volo.Abp.Application.Services;
using Volo.Abp.Auditing;

namespace AISmart.Service;

[DisableAuditing]
public class EventAppService : ApplicationService, IEventAppService
{
    private readonly IClusterClient _clusterClient;

    public EventAppService(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public async Task PublishEventToGroupAsync(Guid groupId, string eventType, object eventData)
    {
        var groupGAgent = _clusterClient.GetGrain<IStateAgent<GroupAgentState>>(groupId);
        var publishingAgent = _clusterClient.GetGrain<IPublishingAgent>(Guid.NewGuid());
        await publishingAgent.PublishTo(groupGAgent);
        await publishingAgent.PublishEventAsync((EventBase)eventData);
    }
}