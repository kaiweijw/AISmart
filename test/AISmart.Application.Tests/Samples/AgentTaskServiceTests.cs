using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AISmart.AgentTask;
using AISmart.Application.Grains.Event;
using AISmart.Domain.Grains.Event;
using Orleans;
using Volo.Abp.Modularity;
using Xunit;

namespace AISmart.Samples;

public class AgentTaskServiceTests : AISmartApplicationTestBase
   
{
    private readonly AgentTaskService _agentTaskService;
    private readonly IClusterClient _clusterClient;
    private readonly string _twitterTopic = "twitter";
    private readonly string _telegramTopic = "Telegram";
    private readonly string _gptTopic = "GPT";

    public AgentTaskServiceTests()
    {
        _agentTaskService = GetRequiredService<AgentTaskService>();
        _clusterClient = GetRequiredService<IClusterClient>();
    }
    
    [Fact]
    public async Task Telegram_Template()
    {
        var templateId = Guid.NewGuid();
         await _clusterClient.GetGrain<IEventFlowTemplateGrain>(templateId).CreateEventNode(new EventFlowTemplateDto
        {
            Id = templateId,
            Description = "Telegram",
            AgentTopic = "Telegram",
            Upstream = null,
            Downstreams = null
        });

      var taskId = await _agentTaskService.CreateAgentTaskAsync(templateId, "send Telegram Message");
      AgentTaskDto agentTaskDto = await _agentTaskService.GetAgentTaskDetailAsync(taskId);
    }
    
    [Fact]
    public async Task Initial_Task_Template()
    {
        var telegramTemplateId = Guid.NewGuid();
        await _clusterClient.GetGrain<IEventFlowTemplateGrain>(telegramTemplateId).CreateEventNode(new EventFlowTemplateDto
        {
            Id = telegramTemplateId,
            Description = _telegramTopic,
            AgentTopic = _telegramTopic,
            Upstream = null,
            Downstreams = null
        });
        var twitterTemplateId = Guid.NewGuid();
        await _clusterClient.GetGrain<IEventFlowTemplateGrain>(twitterTemplateId).CreateEventNode(new EventFlowTemplateDto
        {
            Id = twitterTemplateId,
            Description = _twitterTopic,
            AgentTopic = _twitterTopic,
            Upstream = null,
            Downstreams = null
        });
        var gptTemplateId = Guid.NewGuid();
        await _clusterClient.GetGrain<IEventFlowTemplateGrain>(gptTemplateId).CreateEventNode(new EventFlowTemplateDto
        {
            Id = gptTemplateId,
            Description = "gpt",
            AgentTopic = "gpt",
            Upstream = null,
            Downstreams = new List<Guid>()
            {
                telegramTemplateId,twitterTemplateId
            }
        });

        var taskId = await _agentTaskService.CreateAgentTaskAsync(gptTemplateId, "send Telegram Message");
        AgentTaskDto agentTaskDto = await _agentTaskService.GetAgentTaskDetailAsync(taskId);
        if (agentTaskDto.EventResultDictionary.IsNullOrEmpty())
        {
        }
    }
    
    
    public async Task MockTelegram(string topic,CreatedAgentEvent agentEvent)
    {
        if (topic == _telegramTopic)
        {
          var task =  await _agentTaskService.GetAgentTaskDetailAsync(agentEvent.TaskId);
          // telegram execute
          await _agentTaskService.CompletedEventAsync(agentEvent.TaskId, agentEvent.Id, true, null,"send Telegram success");
        }else if (topic == _twitterTopic)
        {
            var task =  await _agentTaskService.GetAgentTaskDetailAsync(agentEvent.TaskId);
            // telegram execute
            await _agentTaskService.CompletedEventAsync(agentEvent.TaskId, agentEvent.Id, true, null,"send Twitter success");
        }else if (topic == _gptTopic)
        {
            var task =  await _agentTaskService.GetAgentTaskDetailAsync(agentEvent.TaskId);
            // telegram execute
            await _agentTaskService.CompletedEventAsync(agentEvent.TaskId, agentEvent.Id, true, null,"send GPT success");
        }


    }
}
