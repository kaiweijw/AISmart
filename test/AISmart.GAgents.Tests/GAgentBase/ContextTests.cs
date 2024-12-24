using AISmart.GAgents.Tests.TestEvents;
using AISmart.GAgents.Tests.TestGAgents;
using Shouldly;

namespace AISmart.GAgents.Tests.GAgentBase;

[Trait("Category", "BVT")]
public class ContextTests : GAgentTestKitBase
{
    [Fact]
    public async Task ContextPipelineText()
    {
        var contextTestGAgent1 = await Silo.CreateGrainAsync<ContextTestGAgent1>(Guid.NewGuid());
        var contextTestGAgent2 = await Silo.CreateGrainAsync<ContextTestGAgent2>(Guid.NewGuid());
        var contextTestGAgent3 = await Silo.CreateGrainAsync<ContextTestGAgent3>(Guid.NewGuid());
        var groupGAgent = await CreateGroupGAgentAsync(contextTestGAgent1, contextTestGAgent2, contextTestGAgent3);
        var publishingGAgent = await CreatePublishingGAgentAsync(groupGAgent);

        await publishingGAgent.PublishEventAsync(new ContextTestEvent1
        {
            ExpectedContext = new Dictionary<string, object?>
            {
                ["Test1"] = "a string value"
            }
        }.WithContext("Test1", "a string value"));

        var contextTestGAgentState = await contextTestGAgent3.GetStateAsync();
        contextTestGAgentState.Success.ShouldBeTrue();
    }
}