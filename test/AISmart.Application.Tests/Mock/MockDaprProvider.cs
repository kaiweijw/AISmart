using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AISmart.AgentTask;
using AISmart.Application.Grains.Event;
using AISmart.Dapr;
using AISmart.Domain.Grains.Event;
using Microsoft.Extensions.DependencyInjection;
using Orleans;

namespace AISmart.Mock;

public class MockDaprProvider :  IDaprProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _twitterTopic = "twitter";
    private readonly string _telegramTopic = "Telegram";
    private readonly string _gptTopic = "GPT";
    public MockDaprProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task PublishEventAsync<T>(string pubsubName, string topicName, T message)
    {
        if (message is CreatedAgentEvent agentEvent)
        {
            if (topicName == _telegramTopic)
            {
                var task =  await _serviceProvider.GetRequiredService<AgentTaskService>().GetAgentTaskDetailAsync(agentEvent.TaskId);
                // telegram execute
                await _serviceProvider.GetRequiredService<AgentTaskService>().CompletedEventAsync(agentEvent,true, null,"send Telegram success");
            }else if (topicName == _twitterTopic)
            {
                var task =  await _serviceProvider.GetRequiredService<AgentTaskService>().GetAgentTaskDetailAsync(agentEvent.TaskId);
                // telegram execute
                await _serviceProvider.GetRequiredService<AgentTaskService>().CompletedEventAsync(agentEvent, true, null,"send Twitter success");
            }else if (topicName == _gptTopic)
            {
                var task =  await _serviceProvider.GetRequiredService<AgentTaskService>().GetAgentTaskDetailAsync(agentEvent.TaskId);
                // telegram execute
                await _serviceProvider.GetRequiredService<AgentTaskService>().CompletedEventAsync(agentEvent, true, null,"send GPT success");
            }
        }
    }
    
   
}