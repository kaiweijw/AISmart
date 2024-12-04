using System;
using System.Threading.Tasks;
using AISmart.AgentTask;
using AISmart.Dapr;
using AISmart.Domain.Grains.Event;
using AISmart.Options;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenAI;

namespace AISmart.Mock;

public class MockDaprProvider : IDaprProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ChatConfigOptions _chatConfigOptions;

    public MockDaprProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _chatConfigOptions = serviceProvider.GetRequiredService<IOptionsSnapshot<ChatConfigOptions>>().Value;
    }

    public async Task PublishEventAsync<T>(string pubsubName, string topicName, T message)
    {
        if (message is not CreatedEvent agentEvent)
            return;
        switch (topicName)
        {
            case DaprConstants.TwitterTopic:
                await _serviceProvider.GetRequiredService<AgentTaskService>()
                    .CompletedEventAsync(agentEvent, true, null, "send Twitter success");
                break;
            case DaprConstants.TelegramTopic:
                // telegram execute
                await _serviceProvider.GetRequiredService<AgentTaskService>()
                    .CompletedEventAsync(agentEvent, true, null, "send Telegram success");
                break;
            case DaprConstants.GptTopic:
                var task = await _serviceProvider.GetRequiredService<AgentTaskService>()
                    .GetAgentTaskDetailAsync(agentEvent.TaskId);
                var assistantAgent = new OpenAIChatAgent(
                        chatClient: new OpenAIClient(_chatConfigOptions.APIKey).GetChatClient(_chatConfigOptions.Model),
                        name: "assistant",
                        systemMessage: "Generate feedback based on input, and the content includes GPT.")
                    .RegisterMessageConnector()
                    .RegisterPrintMessage();

                // talk to the assistant agent
                var reply = await assistantAgent.SendAsync(agentEvent.Param);
                // CompletedEvent
                await _serviceProvider.GetRequiredService<AgentTaskService>()
                    .CompletedEventAsync(agentEvent, true, "", reply.GetContent());
                break;
        }
    }
}