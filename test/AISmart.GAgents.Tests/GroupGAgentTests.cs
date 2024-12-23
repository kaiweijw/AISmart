using AISmart.Application.Grains.Agents.Group;
using AISmart.GAgent.Core;
using AISmart.GAgents.Tests.TestGAgents;
using Orleans.Streams;
using Shouldly;

namespace AISmart.GAgents.Tests;

[Trait("Category", "BVT")]
public class GroupGAgentTests : GAgentTestKitBase
{
    [Fact(DisplayName = "GroupGAgent should be initialized correctly.")]
    public async Task InitGroupGAgentTest()
    {
        var groupGAgent = await Silo.CreateGrainAsync<GroupGAgent>(Guid.NewGuid());

        // GroupGAgent itself will be the publisher.
        var publishers = new GrainState<Dictionary<Guid, StreamIdentity>>();
        await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.PublishersStateName,
            groupGAgent.GetGrainId(),
            publishers);
        publishers.State.Count.ShouldBe(1);
        publishers.State.First().Value.Guid.ShouldBe(groupGAgent.GetPrimaryKey());

        // Subscribers should be empty because no member is registered.
        var subscribers = new GrainState<List<GrainId>>();
        await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.SubscribersStateName,
            groupGAgent.GetGrainId(),
            subscribers);
        subscribers.State.Count.ShouldBe(0);
    }

    [Fact(DisplayName = "Each gAgent's states should be saved correctly after register.")]
    public async Task RegisterTest()
    {
        var naiveTestGAgent = await Silo.CreateGrainAsync<NaiveTestGAgent>(Guid.NewGuid());
        var groupGAgent = await CreateGroupGAgentAsync(naiveTestGAgent);

        // Check member's states from GrainStorage.
        {
            var subscriptions = new GrainState<Dictionary<Guid, StreamIdentity>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.SubscriptionsStateName,
                naiveTestGAgent.GetGrainId(),
                subscriptions);
            subscriptions.State.Count.ShouldBe(1);
            subscriptions.State.First().Value.Guid.ShouldBe(groupGAgent.GetPrimaryKey());

            var publishers = new GrainState<Dictionary<Guid, StreamIdentity>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.PublishersStateName,
                naiveTestGAgent.GetGrainId(),
                publishers);
            publishers.State.Count.ShouldBe(2);
            publishers.State.First().Value.Guid.ShouldBe(naiveTestGAgent.GetPrimaryKey());
            publishers.State.Last().Value.Guid.ShouldBe(groupGAgent.GetPrimaryKey());
        }

        // Check group's states from GrainStorage.
        {
            var publishers = new GrainState<Dictionary<Guid, StreamIdentity>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.PublishersStateName,
                groupGAgent.GetGrainId(),
                publishers);
            publishers.State.Count.ShouldBe(1);
            publishers.State.First().Value.Guid.ShouldBe(groupGAgent.GetPrimaryKey());

            var subscribers = new GrainState<List<GrainId>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.SubscribersStateName,
                groupGAgent.GetGrainId(),
                subscribers);
            subscribers.State.Count.ShouldBe(1);
            subscribers.State.First().ShouldBe(naiveTestGAgent.GetGrainId());
        }
    }

    [Fact(DisplayName = "Each gAgent's states should be saved correctly after unregister.")]
    public async Task UnregisterTest()
    {
        var naiveTestGAgent = await Silo.CreateGrainAsync<NaiveTestGAgent>(Guid.NewGuid());
        var groupGAgent = await CreateGroupGAgentAsync(naiveTestGAgent);

        await groupGAgent.UnregisterAsync(naiveTestGAgent);

        // Check member's states from GrainStorage.
        {
            var subscriptions = new GrainState<Dictionary<Guid, StreamIdentity>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.SubscriptionsStateName,
                naiveTestGAgent.GetGrainId(),
                subscriptions);
            subscriptions.State.Count.ShouldBe(0);

            var publishers = new GrainState<Dictionary<Guid, StreamIdentity>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.PublishersStateName,
                naiveTestGAgent.GetGrainId(),
                publishers);
            // Only itself.
            publishers.State.Count.ShouldBe(1);
        }

        // Check group's states from GrainStorage.
        {
            var publishers = new GrainState<Dictionary<Guid, StreamIdentity>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.PublishersStateName,
                groupGAgent.GetGrainId(),
                publishers);
            // GroupGAgent itself will still be the publisher.
            publishers.State.Count.ShouldBe(1);
            publishers.State.First().Value.Guid.ShouldBe(groupGAgent.GetPrimaryKey());

            var subscribers = new GrainState<List<GrainId>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.SubscribersStateName,
                groupGAgent.GetGrainId(),
                subscribers);
            subscribers.State.Count.ShouldBe(0);
        }
    }

    [Fact(DisplayName = "Multiple gAgents should be registered correctly to one group.")]
    public async Task OneGroupRegisterMultipleGAgentsTest()
    {
        var naiveTestGAgent1 = await Silo.CreateGrainAsync<NaiveTestGAgent>(Guid.NewGuid());
        var naiveTestGAgent2 = await Silo.CreateGrainAsync<NaiveTestGAgent>(Guid.NewGuid());
        var naiveTestGAgent3 = await Silo.CreateGrainAsync<NaiveTestGAgent>(Guid.NewGuid());
        var groupGAgent = await CreateGroupGAgentAsync(naiveTestGAgent1, naiveTestGAgent2, naiveTestGAgent3);

        // Check each member's states from GrainStorage.
        foreach (var gAgent in new List<NaiveTestGAgent> { naiveTestGAgent1, naiveTestGAgent2, naiveTestGAgent3 })
        {
            var subscriptions = new GrainState<Dictionary<Guid, StreamIdentity>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.SubscriptionsStateName,
                gAgent.GetGrainId(),
                subscriptions);
            subscriptions.State.Count.ShouldBe(1);
            subscriptions.State.First().Value.Guid.ShouldBe(groupGAgent.GetPrimaryKey());

            var publishers = new GrainState<Dictionary<Guid, StreamIdentity>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.PublishersStateName,
                gAgent.GetGrainId(),
                publishers);
            publishers.State.Count.ShouldBe(2);
            publishers.State.First().Value.Guid.ShouldBe(gAgent.GetPrimaryKey());
            publishers.State.Last().Value.Guid.ShouldBe(groupGAgent.GetPrimaryKey());
        }

        // Check group's states from GrainStorage.
        {
            var publishers = new GrainState<Dictionary<Guid, StreamIdentity>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.PublishersStateName,
                groupGAgent.GetGrainId(),
                publishers);
            publishers.State.Count.ShouldBe(1);
            publishers.State.First().Value.Guid.ShouldBe(groupGAgent.GetPrimaryKey());

            var subscribers = new GrainState<List<GrainId>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.SubscribersStateName,
                groupGAgent.GetGrainId(),
                subscribers);
            subscribers.State.Count.ShouldBe(3);
        }
    }

    [Fact(DisplayName = "One gAgent should be registered correctly to multiple group.")]
    public async Task MultipleGroupRegisterOneGAgentTest()
    {
        var naiveTestGAgent = await Silo.CreateGrainAsync<NaiveTestGAgent>(Guid.NewGuid());
        var groupGAgent1 = await CreateGroupGAgentAsync(naiveTestGAgent);
        var groupGAgent2 = await CreateGroupGAgentAsync(naiveTestGAgent);
        var groupGAgent3 = await CreateGroupGAgentAsync(naiveTestGAgent);

        // Check member's states from GrainStorage.
        {
            var subscriptions = new GrainState<Dictionary<Guid, StreamIdentity>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.SubscriptionsStateName,
                naiveTestGAgent.GetGrainId(),
                subscriptions);
            subscriptions.State.Count.ShouldBe(3);

            var publishers = new GrainState<Dictionary<Guid, StreamIdentity>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.PublishersStateName,
                naiveTestGAgent.GetGrainId(),
                publishers);
            publishers.State.Count.ShouldBe(4);
        }

        // Check each group's states from GrainStorage.
        foreach (var groupGAgent in new List<GroupGAgent> { groupGAgent1, groupGAgent2, groupGAgent3 })
        {
            var publishers = new GrainState<Dictionary<Guid, StreamIdentity>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.PublishersStateName,
                groupGAgent.GetGrainId(),
                publishers);
            publishers.State.Count.ShouldBe(1);
            publishers.State.First().Value.Guid.ShouldBe(groupGAgent.GetPrimaryKey());

            var subscribers = new GrainState<List<GrainId>>();
            await Silo.TestGrainStorage.ReadStateAsync(AISmartGAgentConstants.SubscribersStateName,
                groupGAgent.GetGrainId(),
                subscribers);
            subscribers.State.Count.ShouldBe(1);
            subscribers.State.First().ShouldBe(naiveTestGAgent.GetGrainId());
        }
    }
}