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
using AISmart.Provider;
using AutoGen.Core;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Agent;

[Description("micro AI")]
[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class MicroAIGAgent : GAgentBase<MicroAIGAgentState, AIMessageGEvent>, IMicroAIGAgent
{
    private readonly ILogger<MicroAIGAgent> _logger;
    private readonly IChatAgentProvider _chatAgentProvider;

    public MicroAIGAgent(ILogger<MicroAIGAgent> logger,IChatAgentProvider chatAgentProvider) : base(logger)
    {
        _logger = logger;
        _chatAgentProvider = chatAgentProvider;
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
        var history = ConvertMessage(State.RecentMessages.ToList());
        var message =  await _chatAgentProvider.SendAsync(State.AgentName, State.AgentResponsibility, history);
        if (message!= null )
        {
            list.Add(new AIReplyMessageGEvent()
            {
                Message = new MicroAIMessage("assistant", message.GetContent()!)
            });
            aiResponseEvent.Content = message.GetContent()!;
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
        _chatAgentProvider.SetAgent(agentName,agentResponsibility);
    }
    
    private List<IMessage> ConvertMessage(List<MicroAIMessage> listAutoGenMessage)
    {
        var result = new List<IMessage>();
        foreach (var item in listAutoGenMessage)
        {
            result.Add(new TextMessage(GetRole(item.Role), item.Content));
        }

        return result;
    }
    
    private Role GetRole(string roleName)
    {
        switch (roleName)
        {
            case "user":
                return Role.User;
            case "assistant":
                return Role.Assistant;
            case "system":
                return Role.System;
            case "function":
                return Role.Function;
            default:
                return Role.User;
        }
    }
}

public interface IMicroAIGAgent: IStateGAgent<MicroAIGAgentState>
{
    Task SetAgent(string agentName, string agentResponsibility);
}