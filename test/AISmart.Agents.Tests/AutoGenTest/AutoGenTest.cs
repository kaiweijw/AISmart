using AISmart.Agents.AutoGen;
using AISmart.Application.Grains.Agents.Group;
using AISmart.Application.Grains.Agents.Publisher;
using AISmart.GAgent.Autogen;
using AISmart.GAgent.Autogen.Events;
using AISmart.Sender;
using Orleans.TestKit;
using Orleans.TestKit.Streams;

namespace AISmart.Grains.Tests.AutoGenTest;

public class AutoGenTest : TestKitBase
{
    [Fact]
    public async Task AutogenGAgentTest()
    {
        var guid = Guid.NewGuid();
        var groupGAgent = await Silo.CreateGrainAsync<GroupGAgent>(Guid.NewGuid());
        var autogenGAgent = await Silo.CreateGrainAsync<AutogenGAgent>(guid);
        var publishingGAgent = await Silo.CreateGrainAsync<PublishingGAgent>(guid);
        var drawGAgent = await Silo.CreateGrainAsync<DrawOperationGAgent>(guid);
        var mathGAgent = await Silo.CreateGrainAsync<MathOperationGAgent>(guid);
        var autoGenExecutor = await Silo.CreateGrainAsync<AutoGenExecutor>(guid);

        autogenGAgent.RegisterAgentEvent(typeof(DrawOperationGAgent), [typeof(DrawTriangleEvent)]);
        autogenGAgent.RegisterAgentEvent(typeof(MathOperationGAgent), [typeof(AddNumberEvent), typeof(SubNumberEvent)]);

        await groupGAgent.Register(autogenGAgent);
        await groupGAgent.Register(drawGAgent);
        await groupGAgent.Register(mathGAgent);

        Silo.AddProbe<IPublishingGAgent>(_ => publishingGAgent);
        Silo.AddProbe<IAutoGenExecutor>(_ => autoGenExecutor);

        await autogenGAgent.SubscribeTo(publishingGAgent);
        await publishingGAgent.PublishTo(autogenGAgent);

        Silo.AddStreamProbe<AutoGenInternalEventBase>();

        await publishingGAgent.PublishEventAsync(new AutoGenCreatedEvent
        {
            Content = "What is 3+3, and then generate the corresponding polygon?"
        });
    }
}