using System.Diagnostics.CodeAnalysis;
using AISmart.Agents;
using AISmart.Agents.A;
using AISmart.Agents.A.Events;
using AISmart.Agents.B.Events;
using AISmart.GAgent.Core;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Application.Grains.Agents.MockA;

[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class MockAGAgent : GAgentBase<AAgentState, AGEvent>
{
    public MockAGAgent(ILogger<MockAGAgent> logger) : base(logger) { }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when an A thread is published.");
    }

    [EventHandler]
    public async Task HandleEventAsync(AThreadCreatedEvent eventData)
    {
        Logger.LogInformation($"{GetType()} ExecuteAsync: AAgent analyses content: {eventData.Content}");

        State.Number += 1;

        if (State.ThreadIds == null)
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