using System;
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
    public async Task Initial_Task_Template()
    {
        var templateId = Guid.NewGuid();
         await _clusterClient.GetGrain<IEventNodeGrain>(templateId).CreateEventNode(new EventNodeDto
        {
            Id = templateId,
            Description = "Telegram",
            AgentTopic = "Telegram",
            Upstream = null,
            Downstreams = null
        });

       await _agentTaskService.CreateAgentTaskAsync(templateId, "send Telegram Message");
    }
}
