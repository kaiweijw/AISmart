using System.ComponentModel;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using AISmart.Agents;
using AISmart.Agents.AutoGen;
using AISmart.Rag;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using Volo.Abp.Identity;
using IAgent = AutoGen.Core.IAgent;

namespace AISmart.Application.Grains.Agents.AutoGen;

public class AutogenGAgent : GAgent<AutoGenAgentState, AutoGenCreatedEvent>
{
    private readonly IRagProvider _ragProvider;
    private readonly AutoGenExecutor _executor;

    public AutogenGAgent(ILogger logger, IClusterClient clusterClient, AutoGenExecutor executor,
        IRagProvider ragProvider) : base(logger, clusterClient)
    {
        _ragProvider = ragProvider;
        _executor = executor;
    }

    public override Task<string> GetDescriptionAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task ExecuteAsync(AutoGenCreatedEvent eventData)
    {
        List<IMessage> history = new List<IMessage>();
        var ragResponse = await _ragProvider.RetrieveAnswerAsync(eventData.Content);
        if (ragResponse.IsNullOrEmpty() == false)
        {
            history.Add(new TextMessage(Role.System, ragResponse));
        }

        history.Add(new TextMessage(Role.User, eventData.Content));

        Task.Run(async () =>
        {
            await _executor.ExecuteTask(eventData.EventId, history);
        });
    }

    protected override async Task CompleteAsync(AutoGenCreatedEvent eventData)
    {
        var history = State.ChatHistory;
        history.Add(new TextMessage(Role.User, eventData.Content) { From = _agentName });

        await CoordinationAsync(history, eventData);
    }

    private async Task CoordinationAsync(List<IMessage> history, AutoGenCreatedEvent eventData)
    {
        
    }
}