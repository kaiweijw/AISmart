using System.ComponentModel;
using AISmart.Agents;
using Orleans;

namespace AISmart.GEvents.MicroAI;
[Description("Receive a message from telegram.")]
[GenerateSerializer]
public class AIResponseEvent : EventBase
{
    [Description("Details or results of the completed task.")]
    [Id(0)] public string Content { get; set; }
}