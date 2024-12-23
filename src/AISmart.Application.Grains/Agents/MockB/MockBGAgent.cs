using System.Diagnostics.CodeAnalysis;
using AISmart.Agents;
using AISmart.Agents.A.Events;
using AISmart.Agents.B;
using AISmart.Agents.B.Events;
using AISmart.Agents.C.Events;
using AISmart.GAgent.Core;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Application.Grains.Agents.B;

[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class BGAgent : GAgentBase<BAgentState, BGEvent>
{
    public BGAgent(ILogger<BGAgent> logger) : base(logger)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a A thread is published.");
    }

    private Task TryExecuteAsync(BThreadCreatedEvent eventData)
    {
        Logger.LogInformation("TryExecuteAsync: A Thread {AContent}", eventData.Content);
        return Task.CompletedTask;
    }

    [EventHandler]
    public async Task HandleEventAsync(BThreadCreatedEvent eventData)
    {
        Logger.LogInformation($"{GetType()} ExecuteAsync: BAgent analyses content: {eventData.Content}");

        State.Number += 1;

        if (State.ThreadIds.IsNullOrEmpty())
        {
            State.ThreadIds = new List<string>();
        }

        State.ThreadIds.Add(eventData.Id);

        var publishEvent = new CThreadCreatedEvent
        {
            Content = $"A Thread {eventData.Content} has been published."
        };

        await PublishAsync(publishEvent);
        await PublishAsync(new RequestAllSubscriptionsEvent());
    }
}