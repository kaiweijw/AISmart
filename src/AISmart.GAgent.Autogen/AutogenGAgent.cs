using AISmart.Agents.AutoGen;
using AISmart.GAgent.Autogen.State;
using AISmart.Rag;
using AutoGen.Core;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using AISmart.Application.Grains;
using AISmart.GAgent.Autogen.Event;
using AISmart.GAgent.Autogen.EventSourcingEvent;

namespace AISmart.GAgent.Autogen;

[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class AutogenGAgent : GAgent<AutoGenAgentState, BaseEvent>
{
    private readonly IRagProvider _ragProvider;
    private readonly AutoGenExecutor _executor;

    public AutogenGAgent(ILogger logger, IClusterClient clusterClient, AutoGenExecutor executor,
        IRagProvider ragProvider) : base(logger)
    {
        _ragProvider = ragProvider;
        _executor = executor;
    }

    public override Task<string> GetDescriptionAsync()
    {
        throw new NotImplementedException();
    }

    protected override Task ExecuteAsync(BaseEvent eventData)
    {
        throw new NotImplementedException();
    }

    protected override Task CompleteAsync(BaseEvent eventData)
    {
        throw new NotImplementedException();
    }

    protected async Task ExecuteAsync(AutoGenCreatedEvent eventData)
    {
        List<IMessage> history = new List<IMessage>();
        var ragResponse = await _ragProvider.RetrieveAnswerAsync(eventData.Content);
        if (ragResponse.IsNullOrEmpty() == false)
        {
            history.Add(new TextMessage(Role.System, ragResponse));
        }

        history.Add(new TextMessage(Role.User, eventData.Content));

        Task.Run(async () => { await _executor.ExecuteTask(eventData.EventId, history); });

        base.RaiseEvent(new CreateEvent()
        {
            Id = eventData.EventId,
            Messages = history,
            CreateTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
        });
    }

    protected async Task ExecuteAsync(AutoGenSessionFinishedEvent eventData)
    {
        if (eventData.ExecuteStatus == TaskExecuteStatus.Break)
        {
            base.RaiseEvent(new BreakEvent()
            {
                Id = eventData.TaskId,
                BreakReason = eventData.EndContent
            });

            return;
        }

        base.RaiseEvent(new CompleteEvent()
        {
            Id = eventData.TaskId,
            Summary = eventData.EndContent
        });
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        await SubscribeAsync<AutoGenCreatedEvent>(ExecuteAsync);
        await SubscribeAsync<AutoGenSessionFinishedEvent>(ExecuteAsync);
    }
}