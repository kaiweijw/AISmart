using System;
using Orleans;

namespace AISmart.Agent.Event;
[GenerateSerializer]
public  class TransactionFailedGEvent : CreateTransactionGEvent
{
    [Id(0)] public Guid Id { get; set; }
   
}