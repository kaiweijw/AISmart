using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISmart.AgentTask;
using AISmart.Application.Grains.Event;
using AISmart.Dapr;
using AISmart.Domain.Grains.Event;
using AISmart.Mock;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Shouldly;
using Volo.Abp.Modularity;
using Xunit;
using Xunit.Abstractions;

namespace AISmart.Samples;

public class AgentTaskServiceTests : AISmartApplicationTestBase
   
{
    private readonly AgentTaskService _agentTaskService;
    private readonly IClusterClient _clusterClient;
    private readonly string _twitterTopic = "twitter";
    private readonly string _telegramTopic = "Telegram";
    private readonly string _gptTopic = "GPT";
    private readonly ITestOutputHelper _output;
    public AgentTaskServiceTests(ITestOutputHelper output)
    {
        _agentTaskService = GetRequiredService<AgentTaskService>();
        _clusterClient = GetRequiredService<IClusterClient>();
        _output = output;
    }
    
    [Fact]
    public async Task Telegram_Test()
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
      agentTaskDto.EventResultDictionary.ShouldNotBeEmpty();
      agentTaskDto.EventResultDictionary.Values.Select(o => o.Result).ShouldContain("send Telegram success");
    }
    
    [Fact]
    public async Task muti_Task_Test()
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
            Description = _gptTopic,
            AgentTopic = _gptTopic,
            Upstream = null,
            Downstreams = new List<Guid>()
            {
                telegramTemplateId,twitterTemplateId
            }
        });

        var taskId = await _agentTaskService.CreateAgentTaskAsync(gptTemplateId, "send GPT Message");
        AgentTaskDto agentTaskDto = await _agentTaskService.GetAgentTaskDetailAsync(taskId);
       
        agentTaskDto.EventResultDictionary.ShouldNotBeEmpty();
        foreach (var value in agentTaskDto.EventResultDictionary.Values)
        {
            _output.WriteLine(value.AgentTopic +"  " + value.Result);
        }
        agentTaskDto.EventResultDictionary.Values.Select(o => o.Result).ShouldContain("send Twitter success");
        agentTaskDto.EventResultDictionary.Values.Select(o => o.Result).ShouldContain("send Telegram success");
    }
    [Fact]
    public async Task _Task_Test()
    {
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
            Description = _gptTopic,
            AgentTopic = _gptTopic,
            Upstream = null,
            Downstreams = new List<Guid>()
            {
                twitterTemplateId
            }
        });

        var taskId = await _agentTaskService.CreateAgentTaskAsync(gptTemplateId, "比特币突破 10 万美金大关了");
        AgentTaskDto agentTaskDto = await _agentTaskService.GetAgentTaskDetailAsync(taskId);
       
        agentTaskDto.EventResultDictionary.ShouldNotBeEmpty();
        foreach (var value in agentTaskDto.EventResultDictionary.Values)
        {
            _output.WriteLine(value.AgentTopic +"  " + value.Result);
        }
        agentTaskDto.EventResultDictionary.Values.Select(o => o.Result).ShouldContain("send Twitter success");
    }
}
