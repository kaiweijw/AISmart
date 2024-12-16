using AISmart.Agents.ImplementationAgent.Events;
using AISmart.Application.Grains.Agents.MarketLeader;
using AISmart.Application.Grains.Agents.X;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Application.Grains.Agents.Developer;

[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class DeveloperAgent : GAgent<DeveloperAgentState, ImplementationEvent>, IDeveloperAgent
{
    public DeveloperAgent(ILogger<DeveloperAgent> logger, IClusterClient clusterClient) : base(logger)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a social event is published.");
    }

    protected override Task ExecuteAsync(ImplementationEvent eventData)
    {
        Logger.LogInformation($"{this.GetType().ToString()} ExecuteAsync: DeveloperAgent analyses content:{eventData.Content}");
        if (State.Content.IsNullOrEmpty())
        {
            State.Content = [];
        }
        State.Content.Add(eventData.Content);
        return Task.CompletedTask;
    }

    protected override Task CompleteAsync(ImplementationEvent eventData)
    {
        return Task.CompletedTask;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        GrainTracker.DeveloperAgents.Enqueue(this);
        await base.OnActivateAsync(cancellationToken);
        await SubscribeAsync<ImplementationEvent>(ExecuteAsync);
    }

    public async Task ExecuteTransactionAsync(ImplementationEvent gEventData)
    {
        RaiseEvent(gEventData);
        await ConfirmEvents();
        await ExecuteAsync(gEventData);
    }
    
    protected override void OnStateChanged()
    {
        var type = State.GetType();
        Mediator = ServiceProvider.GetRequiredService<IMediator>();
        var command = new DeveloperAgentCommand
        {
            State = State
        };

        Mediator.Send(command);
        base.OnStateChanged();   
    }
}
public interface IDeveloperAgent : IGrainWithGuidKey
{ 
    Task ExecuteTransactionAsync(ImplementationEvent gEventData);
}