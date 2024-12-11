using AISmart.Dapr;
using AISmart.Sender;
using Microsoft.Extensions.Logging;

namespace AISmart.Application.Grains.Agents.Sender;

[GenerateSerializer]
public class PublishingAgentState
{
}

public class EmptyEvent
{
}

public class PublishingAgent : GAgent<PublishingAgentState, EmptyEvent>, IPublishingAgent
{
    public PublishingAgent(ILogger<PublishingAgent> logger) : base(logger)
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

    public async Task PublishEventAsync<T>(T @event)
    {
        await PublishAsync(@event);
    }
}