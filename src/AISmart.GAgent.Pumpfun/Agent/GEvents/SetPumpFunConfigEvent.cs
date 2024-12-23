using Orleans;

namespace AISmart.Agent.GEvents;

[GenerateSerializer]
public class SetPumpFunConfigEvent : PumpFunMessageGEvent
{
    [Id(0)] public string ChatId { get; set; }
    
    [Id(1)] public string BotName { get; set; }
}