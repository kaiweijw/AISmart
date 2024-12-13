using System.Text.Json;
using System.Text.Json.Serialization;
using AISmart.GAgent.Autogen.Common;
using AISmart.GAgent.Autogen.Event;
using AISmart.GAgent.Autogen.EventSourcingEvent;
using AISmart.Rag;
using AISmart.Sender;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using Castle.Core.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using Volo.Abp.DependencyInjection;

namespace AISmart.GAgent.Autogen;

public class AutoGenExecutor : ISingletonDependency
{
    private readonly AgentDescriptionManager _agentDescriptionManager;
    private readonly IClusterClient _clusterClient;
    private readonly ILogger<AutoGenExecutor> _logger;
    private readonly Guid _publishGrainId = new Guid(); 
    private const string AgentName = "admin";
    private const string FinishFlag = "complete";
    private const string BreakFlag = "break";

    public AutoGenExecutor(ILogger<AutoGenExecutor> logger,IClusterClient clusterClient, AgentDescriptionManager agentDescriptionManager)
    {
        _logger = logger;
        _clusterClient = clusterClient;
        _agentDescriptionManager = agentDescriptionManager;
    }

    public async Task ExecuteTask(Guid taskId, List<IMessage> history)
    {
        var chatClient = _clusterClient.ServiceProvider.GetRequiredService<ChatClient>();
        IAgent agent = new OpenAIChatAgent(chatClient, AgentName, GetAgentResponsibility())
            .RegisterMessageConnector()
            .RegisterMiddleware(GetMiddleware());

        var response = await agent.SendAsync("What should be done next?", history);
        var responseStr = response.GetContent();
        if (responseStr.IsNullOrEmpty())
        {
            _logger.LogDebug($"[AutoGenExecutor] autoGen response is null, History:{JsonSerializer.Serialize(history)}");
            return;
        }
        if (responseStr.Contains(FinishFlag))
        {
            var completeEvent = JsonSerializer.Deserialize<CompleteEvent>(responseStr);
            if (completeEvent != null)
            {
                var publishGrain = _clusterClient.GetGrain<IPublishingAgent>(_publishGrainId);
                await publishGrain.PublishEventAsync(new AutoGenSessionFinishedEvent()
                {
                    TaskId = taskId,
                    ExecuteStatus = TaskExecuteStatus.Finish,
                    EndContent = completeEvent.Summary,
                });
                return;
            }
            
            _logger.LogDebug($"[AutoGenExecutor] response is finished,but Deserialize error, History:{JsonSerializer.Serialize(history)} response:{responseStr}");
        }
        else if (responseStr.Contains(BreakFlag))
        {
            var breakInfo = JsonSerializer.Deserialize<EventBreak>(responseStr);
            if (breakInfo != null)
            { 
                var publishGrain = _clusterClient.GetGrain<IPublishingAgent>(_publishGrainId);
                await publishGrain.PublishEventAsync(new AutoGenSessionFinishedEvent()
                {
                    TaskId = taskId,
                    ExecuteStatus = TaskExecuteStatus.Break,
                    EndContent = breakInfo.Break,
                });
                return;
            }
            
            _logger.LogDebug($"[AutoGenExecutor] response is break,but Deserialize error, History:{JsonSerializer.Serialize(history)} response:{responseStr}");
        }
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
                 - The response for each event will be added to the conversation in JSON format. 
                   You need to analyze the response information to decide whether to proceed to the next round. 
                   The response information will be used during the final summary. The JSON format is as follows:
                   {
                    "EventName":"", 
                    "Reply":"",
                   }
                   JSON explanation:EventName is the name of the event,and Reply is the response from the event corresponding to EventName.
                   
                 - If the user's request is completed, please output the Json format:
                    ```
                    {
                        "complete":"{reply summary}"
                    }
                    ```
                    
                 - If the user's request cannot continue, please output the Json format:
                    ```
                    {
                        "break": "{question}"
                    }
                    ```
                 """;
    }

    #region Event hook

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

        //todo: await PublishAsync(eventData as GEvent);

        return JsonSerializer.Serialize(new HandleEventAsyncSchema()
            { EventName = eventName, Parameters = parameters });
    }

    public FunctionContract HandleEventAsyncContract
    {
        get => new FunctionContract
        {
            ClassName = nameof(AutogenGAgent),
            Name = nameof(HandleEventAsync),
            Description = @"Handle Event",
            ReturnType = typeof(Task<string>),
            ReturnDescription = "handle event result",
            Parameters = new global::AutoGen.Core.FunctionParameterContract[]
            {
                new FunctionParameterContract
                {
                    Name = @"eventName",
                    Description = @"event name",
                    ParameterType = typeof(string),
                    IsRequired = true,
                },
                new FunctionParameterContract()
                {
                    Name = @"parameters",
                    Description = @"event parameters",
                    ParameterType = typeof(string),
                    IsRequired = true,
                }
            },
        };
    }

    public Task<string> HandleEventAsyncWrapper(string arguments)
    {
        var schema = JsonSerializer.Deserialize<HandleEventAsyncSchema>(
            arguments,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        return HandleEventAsync(schema.EventName, schema.Parameters);
    }

    public class HandleEventAsyncSchema
    {
        [JsonPropertyName(@"eventName")] public string EventName { get; set; }
        [JsonPropertyName(@"parameters")] public string Parameters { get; set; }
    }

    public class EventBreak
    {
        [JsonPropertyName(@"break")] public string Break { get; set; }
    }

    public class EventComplete
    {
        [JsonPropertyName(@"compete")] public string Complete { get; set; }
    }

    #endregion
}