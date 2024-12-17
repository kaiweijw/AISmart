using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.Developer;
using AISmart.Agents.Group;
using AISmart.Agents.Investment;
using AISmart.Agents.MarketLeader;
using AISmart.Agents.X;
using AISmart.Agents.X.Events;
using AISmart.AgentsNetwork;
using AISmart.Application.Grains;
using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Agents.Group;
using AISmart.Application.Grains.Agents.MarketLeader;
using AISmart.Application.Grains.Agents.X;
using AISmart.Options;
using AISmart.Sender;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Orleans;
using Orleans.TestingHost.Utils;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using ChatConfigOptions = AISmart.Mock.ChatConfigOptions;

namespace AISmart.Samples
{
    public sealed class AINetworkTest : AISmartApplicationTestBase, IAsyncLifetime
    {
        private readonly IGrainFactory _grainFactory;
        
        private readonly IAgentNetworkManager _agentNetworkManager;
        private IPublishingGAgent? _publishingGAgent;
        private readonly AgentNetworkConfigOptions _agentNetworkConfigOptions;


        public AINetworkTest()
        {
            _grainFactory = GetRequiredService<IGrainFactory>();
            _agentNetworkConfigOptions = GetRequiredService<IOptionsSnapshot<AgentNetworkConfigOptions>>().Value;
            _agentNetworkManager = GetRequiredService<IAgentNetworkManager>();
        }

        public Task InitializeAsync()
        {
            _publishingGAgent = _grainFactory.GetGrain<IPublishingGAgent>(Guid.NewGuid());
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            // Clean up resources if needed
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Agent_NetWork_Test()
        {
            _ = _agentNetworkManager.InitializeAgentsAsync(_agentNetworkConfigOptions);

            var agentDictionary = _agentNetworkManager.GetAgentInstances();
            agentDictionary.Count.ShouldBe(7);
            
            var groupAgents = _agentNetworkManager.GetGroupAgents();
            var gAgents = groupAgents as IGAgent[] ?? groupAgents.ToArray();
            gAgents.Length.ShouldBeGreaterThan(0);
            
            var groupStateGAgent = gAgents.FirstOrDefault()!;
            groupStateGAgent.GetType().ShouldBe(typeof(IGAgent));

            _publishingGAgent = _grainFactory.GetGrain<IPublishingGAgent>(Guid.NewGuid());
            await _publishingGAgent.PublishTo(groupStateGAgent);

            const string content = "BTC REACHED 100k WOOHOOOO!";
            var xThreadCreatedEvent = new XThreadCreatedEvent
            {
                Id = "mock_x_thread_id",
                Content = content
            };
            await _publishingGAgent.PublishEventAsync(xThreadCreatedEvent);

            var investmentAgent = GrainTracker.InvestmentAgents.First();
            var investmentAgentState = await investmentAgent.GetStateAsync();
            
            var developerAgent = GrainTracker.DeveloperAgents.First();
            var developerAgentState = await developerAgent.GetStateAsync();

            await TestingUtils.WaitUntilAsync(_ => CheckState(developerAgentState), TimeSpan.FromSeconds(20));
            await TestingUtils.WaitUntilAsync(_ => CheckState(investmentAgentState), TimeSpan.FromSeconds(20));
            
            developerAgentState.Content.Count.ShouldBe(1);
            investmentAgentState.Content.Count.ShouldBe(1);
        }

        private static Task<bool> CheckState(InvestmentAgentState state)
        {
            return Task.FromResult(!CollectionUtilities.IsNullOrEmpty(state.Content));
        }
        
        private static Task<bool> CheckState(DeveloperAgentState state)
        {
            return Task.FromResult(!CollectionUtilities.IsNullOrEmpty(state.Content));
        }
    }
}