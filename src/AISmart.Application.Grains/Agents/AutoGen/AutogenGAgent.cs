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
        State.ChatHistory = history;

        // todo: define IAgent to ConfigureServices
        // todo: middleware is delegate, this grain object will be hold by IAgent.
        IAgent agent = new OpenAIChatAgent(_chatClient, _agentName, GetAgentResponsibility())
            .RegisterMessageConnector()
            .RegisterMiddleware(GetMiddleware());

        var response = await agent.SendAsync("What should be done next?", history);
        var responseStr = response.GetContent();
        var ifContinue = true;
        if (responseStr == _complateFlag)
        {
            State.SessionStateEnum = SessionStateEnum.Completed;
            ifContinue = false;
        }
        else if (responseStr.Contains("break"))
        {
            var breakInfo = JsonSerializer.Deserialize<AutoGenExecutor.EventBreak>(responseStr);
            if (breakInfo != null)
            {
                State.SessionStateEnum = SessionStateEnum.Break;
                ifContinue = false;
            }
        }
        else
        {
            // record current call agent and parameter
            State.CurrentCall = response.GetContent();
        }

        if (ifContinue == false)
        {
            // todo: generate output by call autogen
            // todo: send ack to caller.
        }

        // base.DoAckAsync()
        base.RaiseEvent(eventData);
    }

    private FunctionCallMiddleware GetMiddleware()
    {
        var groupChatContract = new List<FunctionContract>()
        {
            HandleEventAsyncContract
        };

        var groupChatFunctionMap = new Dictionary<string, Func<string, Task<string>>>()
        {
            { nameof(HandleEventAsync), HandleEventAsyncWrapper }
        };

        return new FunctionCallMiddleware(groupChatContract, groupChatFunctionMap);
    }

    private string GetAgentResponsibility()
    {
        return $$"""
                 You are a manager who solves user problems by organizing agents,
                 - The events and their parameters are described as follows Json:
                 {{_agentDescriptionManager.GetAutoGenEventDescription()}}

                 - You can understand what the event can do through the event description, and know the type, name, and description of each parameter through the event parameters. 
                 If the event description can handle the user's request, you need to assemble the request into the following JSON format:
                 {
                     EventName:""
                     Parameters:""
                 }
                 JSON explanation: EventName is the name of the event, and Parameters is a JSON string consisting of the parameter names and values corresponding to the event.

                 The workflow is as follows:
                 - You take the problem from user.
                 - Split the task into different events, and use the output of the previous event combined with the user's request to determine the execution of the next event.
                 - Based on the event's response, reorganize the next request in line with the user's intent until the user's issue is resolved.
                 - If the above events cannot meet the user's task execution needs, you can generate results based on the events to drive the continuation of the process.
                 - If the user's request is completed, please output the string "{{_complateFlag}}".
                 - If the user's request cannot continue, please output the Json format:
                    ```
                    {
                        "break": "{question}"
                    }
                    ```
                 """;
    }

    /// <summary>
    /// handle middleware event
    /// </summary>
    /// <param name="eventName">event name</param>
    /// <param name="parameters">event parameter json string</param>
    /// <returns></returns>
    [Function]
    public async Task<string> HandleEventAsync(string eventName, string parameters)
    {
        var descriptionDic = _agentDescriptionManager.GetAgentDescription();
        if (descriptionDic.TryGetValue(eventName, out var eventDescription) == false)
        {
            throw new Exception($"Event name:{eventName}");
        }

        var eventData = JsonSerializer.Deserialize(parameters, eventDescription.AgentEventType);
        if (eventData == null)
        {
            throw new Exception($"Event name:{eventName} Deserialize object is null, the parameters is{parameters}");
        }

        await PublishAsync(eventData as GEvent);

        return JsonSerializer.Serialize(new AutoGenExecutor.HandleEventAsyncSchema()
            { EventName = eventName, Parameters = parameters });
    }

    public Task<string> HandleEventAsyncWrapper(string arguments)
    {
        var schema = JsonSerializer.Deserialize<AutoGenExecutor.HandleEventAsyncSchema>(
            arguments,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        return HandleEventAsync(schema.EventName, schema.Parameters);
    }
}