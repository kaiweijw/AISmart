using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AISmart.Agent.GEvents;
using AISmart.Agents;
using AISmart.Application.Grains;
using AISmart.Events;
using AISmart.GEvents.MicroAI;
using AISmart.Grains;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Agent;

[Description("micro AI")]
[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class MicroAIGAgent : GAgentBase<MicroAIGAgentState, AIMessageGEvent>, IMicroAIGAgent
{
    private readonly ILogger<MicroAIGAgent> _logger;

    public MicroAIGAgent(ILogger<MicroAIGAgent> logger) : base(logger)
    {
        _logger = logger;
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult(
            "Represents an agent responsible for informing other agents when a micro AI thread is published.");
    }


    [EventHandler]
    public async Task<AIResponseEvent> HandleEventAsync(AIReceiveMessageEvent @event)
    {
        _logger.LogInformation("micro AI ReceiveMessageEvent " + @event.MessageId);
        List<AIMessageGEvent> list = new List<AIMessageGEvent>();
        list.Add( new AIReceiveMessageGEvent
        {
            Message = new MicroAIMessage("user",@event.Message)
        });

        AIResponseEvent aiResponseEvent = new AIResponseEvent();
       
        var message =  await GrainFactory.GetGrain<IChatAgentGrain>(State.AgentName).SendAsync( @event.Message, State.RecentMessages.ToList());
        if (message!= null && !message.Content.IsNullOrEmpty())
        {
            _logger.LogInformation("micro AI replyMessage:" + message.Content);
            list.Add(new AIReplyMessageGEvent()
            {
                Message = message
            });
            aiResponseEvent.Content = message.Content;
            await PublishAsync(new SendMessageEvent
            {
                Message = State.AgentName + ": " +message.Content
            });
        }
        RaiseEvents(list);
        await ConfirmEvents();
        return aiResponseEvent;
    }

    public async Task SetAgent(string agentName, string agentResponsibility)
    {
        
        RaiseEvent(new AISetAgentMessageGEvent
        {
            AgentName = agentName,
            AgentResponsibility = agentResponsibility
        });
        await ConfirmEvents();
        await GrainFactory.GetGrain<IChatAgentGrain>(agentName).SetAgentAsync(agentResponsibility);
    }

    public async Task<MicroAIGAgentState> GetAgentState()
    {
        return State;
    }
}

public interface IMicroAIGAgent: IStateGAgent<MicroAIGAgentState>
{
    Task SetAgent(string agentName, string agentResponsibility);
    
    Task<MicroAIGAgentState> GetAgentState();
}