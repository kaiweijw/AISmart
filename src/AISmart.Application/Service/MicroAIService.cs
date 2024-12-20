using System;
using System.Threading.Tasks;
using AISmart.Agent;
using AISmart.Agents;
using AISmart.Agents.Group;
using AISmart.Events;
using AISmart.GAgent.Autogen;
using AISmart.GEvents.MicroAI;
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
        var telegramAgent = _clusterClient.GetGrain<ITelegramGAgent>(Guid.NewGuid());
        await telegramAgent.SetTelegramConfig("-1002473003637", "Test");
       
        var autogenAgent=  _clusterClient.GetGrain<IAutogenGAgent>(Guid.NewGuid());
        autogenAgent.RegisterAgentEvent(typeof(TelegramGAgent), [typeof(ReceiveMessageEvent), typeof(SendMessageEvent)]);
        autogenAgent.RegisterAgentEvent(typeof(MicroAIGAgent), [typeof(AIReceiveMessageEvent)]);
        
        await groupAgent.Register(telegramAgent);
        await groupAgent.Register(autogenAgent);
        for (int a =0 ; a< 7; a++) {
            var microAIGAgent = _clusterClient.GetGrain<IMicroAIGAgent>(Guid.NewGuid());
            await microAIGAgent.SetAgent("TestAI"+(a+1),"You are a voter. Based on a proposal, provide a conclusion of agreement or disagreement and give reasons.");
            await groupAgent.Register(microAIGAgent);
        }
        
        var publishingAgent = _clusterClient.GetGrain<IPublishingGAgent>(PublishId);
        await publishingAgent.PublishTo(groupAgent);

        await publishingAgent.PublishEventAsync(new RequestAllSubscriptionsEvent());
    }
}