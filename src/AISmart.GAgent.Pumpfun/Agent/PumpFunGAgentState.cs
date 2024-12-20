using System;
using System.Collections.Generic;
using AISmart.Agent.GEvents;
using Orleans;

namespace AISmart.Agent;

public class PumpFunGAgentState 
{
    [Id(0)] public Guid Id { get; set; } = Guid.NewGuid();
    
    [Id(1)] public Dictionary<string, PumpFunReceiveMessageGEvent> PendingMessages { get; set; } = new Dictionary<string, PumpFunReceiveMessageGEvent>();
    
    [Id(2)] public string ChatId { get; set; }
    
    [Id(3)] public string BotName { get; set; } 
    
    public void Apply(PumpFunReceiveMessageGEvent receiveMessageGEvent)
    {
        PendingMessages[receiveMessageGEvent.ReplyId] = receiveMessageGEvent;
    }
    
    public void Apply(PumpFunSendMessageGEvent sendMessageGEvent)
    {
        if (!sendMessageGEvent.ReplyId.IsNullOrEmpty())
        {
            PendingMessages.Remove(sendMessageGEvent.ReplyId);
        }
    }
    
    public void Apply(SetPumpFunConfigEvent setPumpFunConfigEvent)
    {
        ChatId = setPumpFunConfigEvent.ChatId;
        BotName = setPumpFunConfigEvent.BotName;
    }

}