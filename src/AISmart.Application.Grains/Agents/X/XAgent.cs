using AISmart.Agents.MarketLeader.Events;
using AISmart.Agents.X.Events;
using AISmart.Dapr;
using Microsoft.Extensions.Logging;

namespace AISmart.Application.Grains.Agents.X;

public class XAgent : GAgent<XAgentState, XThreadCreatedEvent>
{
    public XAgent(ILogger<XAgent> logger,IClusterClient clusterClient) : base(logger,clusterClient)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a X thread is published.");
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);

        await SubscribeAsync<XThreadCreatedEvent>(TryExecuteAsync);
    }

    private Task TryExecuteAsync(XThreadCreatedEvent eventData)
    {
        Logger.LogInformation("TryExecuteAsync: X Thread {XContent}", eventData.Content);
        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(XThreadCreatedEvent eventData)
    {
        Logger.LogInformation($"{this.GetType().ToString()} ExecuteAsync: XAgent analyses content:{eventData.Content}");

        var publishEvent = new SocialEvent
        {
            Content = $"X Thread {eventData.Content} has been published."
        };
        await PublishAsync(publishEvent);
    }

    protected override Task CompleteAsync(XThreadCreatedEvent eventData)
    {
        Logger.LogInformation("CompleteAsync: X Thread {XContent}", eventData.Content);

        return Task.CompletedTask;
    }
}