using System;
using System.Collections.Generic;
using AISmart.Agent.Event;
using Google.Protobuf;
using Orleans;

namespace AISmart.Agent;

[GenerateSerializer]
public class AElfAgentState
{
    [Id(0)]  public  Guid Id { get; set; }
    
    [Id(1)] public Dictionary<Guid, CreateTransactionGEvent> PendingTransactions { get; set; } = new Dictionary<Guid, CreateTransactionGEvent>();
  
}