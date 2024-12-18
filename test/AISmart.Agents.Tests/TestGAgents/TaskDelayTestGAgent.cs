using AISmart.Agents;
using AISmart.Application.Grains;
using AISmart.Grains.Tests.TestEvents;
using Microsoft.Extensions.Logging;

namespace AISmart.Grains.Tests.TestGAgents;

[GenerateSerializer]
public class TaskDelayTestGAgentState
{
    [Id(0)]  public List<string> Contet { get; set; }
}

public class TaskDelayTestGEvent : GEventBase;

public class TaskDelayTestGAgent : GAgentBase<TaskDelayTestGAgentState, TaskDelayTestGEvent>
{
    public TaskDelayTestGAgent(ILogger logger) : base(logger)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("This GAgent is used to test task delay.");
    }
    
    public async Task<NaiveTestEvent> HandleEventAsync(ResponseTestEvent eventData)
    {
        await Task.Delay(1000);
        return new NaiveTestEvent
        {
            Greeting = eventData.Greeting
        };
    }
}