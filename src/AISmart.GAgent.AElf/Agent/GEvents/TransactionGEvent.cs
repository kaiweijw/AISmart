using System;
using Orleans;

namespace AISmart.Agent.GEvents;
[GenerateSerializer]
public class TransactionGEvent :Agents.GEvent
{
    [Id(0)] public Guid Id { get; set; }
}