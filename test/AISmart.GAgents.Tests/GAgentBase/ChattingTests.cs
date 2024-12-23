using AISmart.Application.Grains.Agents.Group;
using AISmart.GAgent.Core;
using AISmart.GAgents.Tests.TestEvents;
using AISmart.GAgents.Tests.TestGAgents;
using Orleans.TestKit;
using Shouldly;

namespace AISmart.GAgents.Tests.GAgentBase;

[Trait("Category", "BVT")]
public class ChattingTests : GAgentTestKitBase
{
    [Fact]
    public async Task ChatIdRelayTest()
    {
        var contextStorageGrain = await Silo.CreateGrainAsync<ContextStorageGrain>(Guid.NewGuid());
        var chatTestGAgent = await Silo.CreateGrainAsync<ChatTestGAgent>(Guid.NewGuid());
        var relayTestGAgent = await Silo.CreateGrainAsync<RelayTestGAgent>(Guid.NewGuid());
        var groupGAgent = await CreateGroupGAgentAsync(chatTestGAgent, relayTestGAgent);
        var publishingGAgent = await CreatePublishingGAgentAsync(groupGAgent);

        Silo.AddProbe<IContextStorageGrain>(_ => contextStorageGrain);
        await publishingGAgent.PublishEventAsync(new ReceiveMessageTestEvent
        {
            MessageId = "Fake message id",
            ChatId = "origin chat id",
            Message = "Hello, world!",
            BotName = "Fake bot name"
        });

        var chatTestGAgentState = await chatTestGAgent.GetStateAsync();
        chatTestGAgentState.SendMessages.Count.ShouldBe(1);
        chatTestGAgentState.SendMessages["origin chat id"].ShouldBe("I handled a social event: Hello, world!");
    }
}