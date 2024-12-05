using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISmart.AgentTask;
using AISmart.Application.Grains.Event;
using AISmart.Dapr;
using AISmart.Domain.Grains.Event;
using Orleans;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AISmart.Samples;

public class AgentTaskServiceTests : AISmartApplicationTestBase

{
    private readonly AgentTaskService _agentTaskService;
    private readonly IClusterClient _clusterClient;
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
        await _clusterClient.GetGrain<IEventFlowTemplateGrain>(templateId).CreateEventNodeAsync(new EventFlowTemplateDto
        {
            Id = templateId,
            AgentTopic = DaprConstants.TelegramTopic,
            Downstreams = null
        });

        var taskId = await _agentTaskService.CreateAgentTaskAsync(templateId, "send Telegram Message");
        AgentTaskDto agentTaskDto = await _agentTaskService.GetAgentTaskDetailAsync(taskId);
        agentTaskDto.EventResultDictionary.ShouldNotBeEmpty();
        agentTaskDto.EventResultDictionary.Values.Select(o => o.Result).ShouldContain("send Telegram success");
    }

    [Fact]
    public async Task Muti_Task_Test()
    {
        var telegramTemplateId = Guid.NewGuid();
        await _clusterClient.GetGrain<IEventFlowTemplateGrain>(telegramTemplateId).CreateEventNodeAsync(
            new EventFlowTemplateDto
            {
                Id = telegramTemplateId,
                AgentTopic = DaprConstants.TelegramTopic,
                Downstreams = null
            });
        var twitterTemplateId = Guid.NewGuid();
        await _clusterClient.GetGrain<IEventFlowTemplateGrain>(twitterTemplateId).CreateEventNodeAsync(
            new EventFlowTemplateDto
            {
                Id = twitterTemplateId,
                AgentTopic = DaprConstants.TwitterTopic,
                Downstreams = null
            });
        var gptTemplateId = Guid.NewGuid();
        await _clusterClient.GetGrain<IEventFlowTemplateGrain>(gptTemplateId).CreateEventNodeAsync(new EventFlowTemplateDto
        {
            Id = gptTemplateId,
            Description = DaprConstants.GptTopic,
            AgentTopic = DaprConstants.GptTopic,
            Downstreams = new List<Guid>()
            {
                telegramTemplateId, twitterTemplateId
            }
        });

        var taskId = await _agentTaskService.CreateAgentTaskAsync(gptTemplateId, "send GPT Message");
        AgentTaskDto agentTaskDto = await _agentTaskService.GetAgentTaskDetailAsync(taskId);

        agentTaskDto.EventResultDictionary.ShouldNotBeEmpty();
        foreach (var value in agentTaskDto.EventResultDictionary.Values)
        {
            _output.WriteLine(value.AgentTopic + "  " + value.Result);
        }

    }

    [Fact]
    public async Task Sequential_Task_Test()
    {
        var twitterTemplateId = Guid.NewGuid();
        await _clusterClient.GetGrain<IEventFlowTemplateGrain>(twitterTemplateId).CreateEventNodeAsync(
            new EventFlowTemplateDto
            {
                Id = twitterTemplateId,
                AgentTopic = DaprConstants.TwitterTopic,
                Downstreams = null
            });
        var gptTemplateId = Guid.NewGuid();
        await _clusterClient.GetGrain<IEventFlowTemplateGrain>(gptTemplateId).CreateEventNodeAsync(new EventFlowTemplateDto
        {
            Id = gptTemplateId,
            AgentTopic = DaprConstants.GptTopic,
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
            _output.WriteLine(value.AgentTopic + "  " + value.Result);
        }
    }
}