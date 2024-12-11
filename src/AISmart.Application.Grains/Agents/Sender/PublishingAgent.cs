using AISmart.Agents;
using AISmart.Sender;
using Microsoft.Extensions.Logging;

namespace AISmart.Application.Grains.Agents.Sender;

[GenerateSerializer]
public class PublishingAgentState
{
}

public class EmptyEvent : GEvent
{
}

public class PublishingAgent : GAgent<PublishingAgentState, EmptyEvent>, IPublishingAgent
{
    public PublishingAgent(ILogger<PublishingAgent> logger, IClusterClient clusterClient) : base(logger, clusterClient)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("Agent to be used for publishing new events.");
    }

    protected override Task ExecuteAsync(EmptyEvent eventData)
    {
        throw new NotImplementedException();
    }

    protected override Task CompleteAsync(EmptyEvent eventData)
    {
        throw new NotImplementedException();
    }

    public async Task PublishEventAsync<T>(T @event) where T : GEvent
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        await PublishAsync(@event);
    }
}