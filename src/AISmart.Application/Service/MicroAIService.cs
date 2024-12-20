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
        var microAIGAgent = _clusterClient.GetGrain<IMicroAIGAgent>(Guid.NewGuid());
        var telegramAgent = _clusterClient.GetGrain<IStateGAgent<TelegramGAgentState>>(Guid.NewGuid());
        await microAIGAgent.SetAgent("TestAI","You are a voter. Based on a proposal, provide a conclusion of agreement or disagreement and give reasons.");

        await groupAgent.Register(microAIGAgent);
        await groupAgent.Register(telegramAgent);
        
        var publishingAgent = _clusterClient.GetGrain<IPublishingGAgent>(PublishId);
        await publishingAgent.PublishTo(groupAgent);
    }
}