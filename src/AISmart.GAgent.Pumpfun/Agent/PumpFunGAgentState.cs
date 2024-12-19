using System;
using System.Collections.Generic;
using AISmart.Agent.GEvents;
using Orleans;

namespace AISmart.Agent;

public class PumpFunGAgentState 
{
    [Id(0)]  public  Guid Id { get; set; }
    
    [Id(1)] public Dictionary<string, PumpFunReceiveMessageGEvent> PendingMessages { get; set; } = new Dictionary<string, PumpFunReceiveMessageGEvent>();
    
    // TODO:把chatId写在
    public void Apply(PumpFunReceiveMessageGEvent receiveMessageGEvent)
    {
        PendingMessages[receiveMessageGEvent.MessageId] = receiveMessageGEvent;
    }
    
    public void Apply(PumpFunSendMessageGEvent sendMessageGEvent)
    {
        if (!sendMessageGEvent.requestMessageId.IsNullOrEmpty())
        {
            PendingMessages.Remove(sendMessageGEvent.requestMessageId);
        }
    }

}