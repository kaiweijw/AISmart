using AISmart.Agents.AutoGen;
using AISmart.Application.Grains.Agents.Group;
using AISmart.Application.Grains.Agents.Publisher;
using AISmart.GAgent.Autogen;
using AISmart.Sender;
using Orleans.TestKit;

namespace AISmart.Grains.Tests.AutoGenTest;

public class AutoGenTest:TestKitBase
{
    [Fact]
    public async Task AutogenGAgentTest()
    {
        var guid = Guid.NewGuid();
        var groupAgent = await Silo.CreateGrainAsync<GroupGAgent>(Guid.NewGuid());
        var autogenGAgent = await Silo.CreateGrainAsync<AutogenGAgent>(guid);
        var publishingAgent = await Silo.CreateGrainAsync<PublishingGAgent>(guid);
        var drawAgent = await Silo.CreateGrainAsync<DrawOperationGAgent>(guid);
        var mathAgent = await Silo.CreateGrainAsync<MathOperationGAgent>(guid);
        
        autogenGAgent.RegisterAgentEvent(typeof(DrawOperationGAgent), [typeof(DrawTriangleEvent)]);
        autogenGAgent.RegisterAgentEvent(typeof(MathOperationGAgent), [typeof(AddNumberEvent), typeof(SubNumberEvent)]);
        
        await groupAgent.Register(autogenGAgent);
        await groupAgent.Register(drawAgent);
        await groupAgent.Register(mathAgent);
        
        Silo.AddProbe<IPublishingGAgent>(_ => publishingAgent);

        await autogenGAgent.SubscribeTo(publishingAgent);
        await publishingAgent.PublishTo(autogenGAgent);
        
        await publishingAgent.PublishEventAsync(new AutoGenCreatedEvent
        {
            Content = "What is 3+3, and then generate the corresponding polygon?"
        });
        
        await Task.Delay(100 * 1000);
    }
}