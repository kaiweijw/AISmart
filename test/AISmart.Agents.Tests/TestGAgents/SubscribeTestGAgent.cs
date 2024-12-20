using AISmart.Agents;
using AISmart.GAgent.Core;
using Microsoft.Extensions.Logging;
using Orleans.Storage;

namespace AISmart.Grains.Tests.TestGAgents;

[GenerateSerializer]
public class SubscribeTestGAgentState
{
    [Id(0)]  public Dictionary<Type, List<Type>> SubscriptionInfo { get; set; }
}

public class SubscribeTestGEvent : GEventBase;

[GAgent]
public class SubscribeTestGAgent : GAgentBase<SubscribeTestGAgentState, SubscribeTestGEvent>
{
    public SubscribeTestGAgent(ILogger logger, IGrainStorage grainStorage) : base(logger, grainStorage)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("This GAgent is used to test implementation of GetAllSubscribedEventsAsync.");
    }
    
    public async Task HandleEventAsync(SubscribedEventListEvent eventData)
    {
        if (State.SubscriptionInfo.IsNullOrEmpty())
        {
            State.SubscriptionInfo = eventData.Value;
        }
    }
}