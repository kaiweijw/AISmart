using System;
using System.Threading.Tasks;
using AISmart.Agent;
using AISmart.Agents;
using AISmart.Agents.Group;
using AISmart.Events;
using AISmart.Sender;
using Microsoft.Extensions.Logging;
using Orleans;
using Volo.Abp.Application.Services;

namespace AISmart.Service;

public class MicroAIService :  ApplicationService,IMicroAIService
{
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<MicroAIService> _logger;
    private static readonly Guid PublishId = Guid.NewGuid();
    
    public MicroAIService(IClusterClient clusterClient,ILogger<MicroAIService> logger)
    {
        _clusterClient = clusterClient;
        _logger = logger;
    }
    public async Task ReceiveMessagesAsync(string message)
    {
        var publishingAgent = _clusterClient.GetGrain<IPublishingGAgent>(PublishId);
        await  publishingAgent.PublishEventAsync(new AIReceiveMessageEvent()
        {
            MessageId = Guid.NewGuid(),
            Message = message
        });
    }

    public async Task SetGroupsAsync()
    {
        var groupAgent = _clusterClient.GetGrain<IStateGAgent<GroupAgentState>>(Guid.NewGuid());
        var microAIGAgent = _clusterClient.GetGrain<IStateGAgent<MicroAIGAgent>>(Guid.NewGuid());
        await groupAgent.Register(microAIGAgent);
        
        var publishingAgent = _clusterClient.GetGrain<IPublishingGAgent>(PublishId);
        await publishingAgent.PublishTo(groupAgent);
        
        var aigAgent = _clusterClient.GetGrain<IMicroAIGAgent>(PublishId);
        aigAgent.SetAgent("TestAI","you are a");
    }
}