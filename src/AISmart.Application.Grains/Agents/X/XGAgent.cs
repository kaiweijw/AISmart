using System.Diagnostics.CodeAnalysis;
using AISmart.Agents;
using AISmart.Agents.MarketLeader.Events;
using AISmart.Agents.X;
using AISmart.Agents.X.Events;
using AISmart.Dapr;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Application.Grains.Agents.X;

[ImplicitStreamSubscription(CommonConstants.StreamNamespace)]
[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class XGAgent : GAgentBase<XAgentState, XGEvent>
{
    public XGAgent(ILogger<XGAgent> logger) : base(logger)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a X thread is published.");
    }

    private Task TryExecuteAsync(XThreadCreatedEvent eventData)
    {
        Logger.LogInformation("TryExecuteAsync: X Thread {XContent}", eventData.Content);
        return Task.CompletedTask;
    }

    private async Task ExecuteAsync(XThreadCreatedEvent eventData)
    {
        Logger.LogInformation($"{GetType()} ExecuteAsync: XAgent analyses content:{eventData.Content}");
        if (State.ThreadIds.IsNullOrEmpty())
        {
            State.ThreadIds = [];
        }

        State.ThreadIds.Add(eventData.Id);

        var publishEvent = new SocialEvent
        {
            Content = $"X Thread {eventData.Content} has been published."
        };
        await PublishAsync(publishEvent);
    }

    public override async Task HandleEventAsync(EventWrapperBase item)
    {
        if (item is EventWrapper<XThreadCreatedEvent> wrapper)
        {
            await ExecuteAsync(wrapper.Event);
        }
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        GrainTracker.XAgents.Enqueue(this);
        await base.OnActivateAsync(cancellationToken);
    }
}