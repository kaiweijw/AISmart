using System;
using Orleans;

namespace AISmart.Agent.GEvents;

[GenerateSerializer]
public class AIMessageGEvent :Agents.GEventBase
{
    [Id(0)] public Guid Id { get; set; } = Guid.NewGuid();
}

[GenerateSerializer]
public class MicroAIMessage
{
    public MicroAIMessage(string role, string content)
    {
        Role = role;
        Content = content;
    }

    [Id(0)]public string Role { get; set; }
    [Id(1)] public string Content { get; set; }
    
    
}