using System.Diagnostics.CodeAnalysis;
using AISmart.Agents;
using AISmart.Agents.A;
using AISmart.Agents.A.Events;
using AISmart.Agents.B.Events;
using AISmart.GAgent.Core;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Application.Grains.Agents.A;

[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class AGAgent : GAgentBase<AAgentState, AGEvent>
{
    public AGAgent(ILogger<AGAgent> logger) : base(logger)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a A thread is published.");
    }

    private Task TryExecuteAsync(AThreadCreatedEvent eventData)
    {
        Logger.LogInformation("TryExecuteAsync: A Thread {AContent}", eventData.Content);
        return Task.CompletedTask;
    }

    [EventHandler]
    public async Task HandleEventAsync(AThreadCreatedEvent eventData)
    {
        Logger.LogInformation($"{GetType()} ExecuteAsync: AAgent analyses content: {eventData.Content}");
        
        for (int i = 0; i < 100; i++)
        {
            State.Number += 1;
            
            if (State.ThreadIds.IsNullOrEmpty())
            {
                State.ThreadIds = new List<string>();
            }
        
            State.ThreadIds.Add(eventData.Id);

            var publishEvent = new BThreadCreatedEvent
            {
                Content = $"A Thread {eventData.Content} has been published."
            };

            await PublishAsync(publishEvent);
            await PublishAsync(new RequestAllSubscriptionsEvent());
        }
    }
}