using AISmart.Agents;
using AISmart.Agents.AutoGen;
using AISmart.GAgent.Autogen.State;
using AutoGen.Core;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using AISmart.Application.Grains;
using AISmart.GAgent.Autogen.Common;
using AISmart.GAgent.Autogen.Event;
using AISmart.GAgent.Autogen.EventSourcingEvent;
using AISmart.Provider;

namespace AISmart.GAgent.Autogen;

[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class AutogenGAgent : GAgentBase<AutoGenAgentState, BaseEvent>, IAutogenAgent
{
    private readonly IRagProvider _ragProvider;
    private readonly AutoGenExecutor _executor;
    private readonly AgentDescriptionManager _agentDescriptionManager;

    public AutogenGAgent(ILogger logger, IClusterClient clusterClient, AutoGenExecutor executor,
        IRagProvider ragProvider, AgentDescriptionManager agentDescriptionManager) : base(logger)
    {
        _ragProvider = ragProvider;
        _executor = executor;
        _agentDescriptionManager = agentDescriptionManager;
    }

    public void RegisterAgentEvent(Type agent, List<Type> eventTypes)
    {
        _agentDescriptionManager.AddAgentEvents(agent, eventTypes);
    }

    public override Task<string> GetDescriptionAsync()
    {
        throw new NotImplementedException();
    }

    [EventHandler]
    public async Task ExecuteAsync(AutoGenCreatedEvent eventData)
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

    [EventHandler]
    public async Task ExecuteAsync(AutoGenExecutorEvent eventData)
    {
        switch (eventData.ExecuteStatus)
        {
            case TaskExecuteStatus.Progressing:
                base.RaiseEvent(new CallerProgressingEvent()
                {
                    Id = eventData.TaskId,
                    CurrentCallInfo = eventData.CurrentCallInfo,
                });
                break;
            case TaskExecuteStatus.Break:
                base.RaiseEvent(new BreakEvent()
                {
                    Id = eventData.TaskId,
                    BreakReason = eventData.EndContent
                });
                break;
            case TaskExecuteStatus.Finish:
                base.RaiseEvent(new CompleteEvent()
                {
                    Id = eventData.TaskId,
                    Summary = eventData.EndContent
                });
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}