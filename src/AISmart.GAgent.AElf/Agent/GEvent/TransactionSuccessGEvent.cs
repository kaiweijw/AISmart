using System;
using AISmart.Agent.Event;
using Orleans;

namespace AISmart.Agent.GEvent;

public  class TransactionSuccessGEvent : CreateTransactionGEvent
{
    [Id(0)] public Guid Id { get; set; } = Guid.NewGuid();
    [Id(1)] public string TransactionId { get; set; }
}