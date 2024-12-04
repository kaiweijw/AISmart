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
      TaskDto taskDto = await _agentTaskService.GetAgentTaskDetailAsync(taskId);
    }
    
    [Fact]
    public async Task Initial_Task_Template()
    {
        var telegramTemplateId = Guid.NewGuid();
        await _clusterClient.GetGrain<IEventFlowTemplateGrain>(telegramTemplateId).CreateEventNode(new EventFlowTemplateDto
        {
            Id = telegramTemplateId,
            Description = "Telegram",
            AgentTopic = "Telegram",
            Upstream = null,
            Downstreams = null
        });
        var twitterTemplateId = Guid.NewGuid();
        await _clusterClient.GetGrain<IEventFlowTemplateGrain>(twitterTemplateId).CreateEventNode(new EventFlowTemplateDto
        {
            Id = twitterTemplateId,
            Description = "Twitter",
            AgentTopic = "Twitter",
            Upstream = null,
            Downstreams = null
        });
        var gptTemplateId = Guid.NewGuid();
        await _clusterClient.GetGrain<IEventFlowTemplateGrain>(gptTemplateId).CreateEventNode(new EventFlowTemplateDto
        {
            Id = gptTemplateId,
            Description = "Twitter",
            AgentTopic = "Twitter",
            Upstream = null,
            Downstreams = new List<Guid>()
            {
                telegramTemplateId,twitterTemplateId
            }
        });

        var taskId = await _agentTaskService.CreateAgentTaskAsync(gptTemplateId, "send Telegram Message");
        TaskDto taskDto = await _agentTaskService.GetAgentTaskDetailAsync(taskId);
    }
}
