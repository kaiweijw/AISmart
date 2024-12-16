using AISmart.Agents;
using AISmart.Agents.Developer;
using AISmart.Agents.ImplementationAgent.Events;
using AISmart.Dapr;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Application.Grains.Agents.Developer;

[ImplicitStreamSubscription(CommonConstants.StreamNamespace)]
[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class DeveloperGAgent : GAgentBase<DeveloperAgentState, DeveloperGEvent>
{
    public DeveloperGAgent(ILogger<DeveloperGAgent> logger) : base(logger)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a social event is published.");
    }

    private Task ExecuteAsync(ImplementationEvent eventData)
    {
        Logger.LogInformation($"{this.GetType().ToString()} ExecuteAsync: DeveloperAgent analyses content:{eventData.Content}");
        if (State.Content.IsNullOrEmpty())
        {
            State.Content = [];
        }
        State.Content.Add(eventData.Content);
        return Task.CompletedTask;
    }

    public override async Task HandleEvent(EventWrapperBase item)
    {
        if (item is EventWrapper<ImplementationEvent> wrapper)
        {
            await ExecuteAsync(wrapper.Event);
        }
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        GrainTracker.DeveloperAgents.Enqueue(this);
        await base.OnActivateAsync(cancellationToken);
    }
}