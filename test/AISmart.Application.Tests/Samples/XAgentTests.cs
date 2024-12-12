using System;
using System.Linq;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.X.Events;
using AISmart.Application.Grains;
using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Agents.MarketLeader;
using AISmart.Application.Grains.Agents.X;
using AISmart.Sender;
using Microsoft.IdentityModel.Tokens;
using Orleans;
using Orleans.TestingHost.Utils;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AISmart.Samples
{
    public class XAgentTests : AISmartApplicationTestBase, IAsyncLifetime
    {
        private readonly IClusterClient _clusterClient;
        protected readonly IGrainFactory _grainFactory;

        private IAgent<XAgentState> _xAgent;
        private IAgent<MarketLeaderAgentState> _marketLeaderAgent;
        private IAgent<DeveloperAgentState> _developerAgent;
        private IAgent<InvestmentAgentState> _investmentAgent;
        private IPublishingAgent _publishingAgent;

        public XAgentTests(ITestOutputHelper output)
        {
            _clusterClient = GetRequiredService<IClusterClient>();
            _grainFactory = GetRequiredService<IGrainFactory>();
        }

        public async Task InitializeAsync()
        {
            _xAgent = _grainFactory.GetGrain<IAgent<XAgentState>>(Guid.NewGuid());
            await _xAgent.ActivateAsync();

            _marketLeaderAgent = _grainFactory.GetGrain<IAgent<MarketLeaderAgentState>>(Guid.NewGuid());
            await _marketLeaderAgent.ActivateAsync();

            _developerAgent = _grainFactory.GetGrain<IAgent<DeveloperAgentState>>(Guid.NewGuid());
            await _developerAgent.ActivateAsync();

            _investmentAgent = _grainFactory.GetGrain<IAgent<InvestmentAgentState>>(Guid.NewGuid());
            await _investmentAgent.ActivateAsync();

            _publishingAgent = _clusterClient.GetGrain<IPublishingAgent>(Guid.NewGuid());
        }

        public Task DisposeAsync()
        {
            // Clean up resources if needed
            return Task.CompletedTask;
        }

        [Fact]
        public async Task XThreadCreatedEvent_Executed_Test()
        {
            const string content = "BTC REACHED 100k WOOHOOOO!";

            var xThreadCreatedEvent = new XThreadCreatedEvent
            {
                Id = "mock_x_thread_id",
                Content = content
            };

            await _publishingAgent.PublishEventAsync(xThreadCreatedEvent);

            var investmentAgent = GrainTracker.InvestmentAgents.First();
            var investmentAgentState = await investmentAgent.GetStateAsync();
            
            var developerAgent = GrainTracker.DeveloperAgents.First();
            var developerAgentState = await developerAgent.GetStateAsync();

            await TestingUtils.WaitUntilAsync(_ => CheckState(developerAgentState), TimeSpan.FromSeconds(20));
            await TestingUtils.WaitUntilAsync(_ => CheckState(investmentAgentState), TimeSpan.FromSeconds(20));
            
            developerAgentState.Content.Count.ShouldBe(1);
            investmentAgentState.Content.Count.ShouldBe(1);
        }

        private async Task<bool> CheckState(InvestmentAgentState state)
        {
            return !state.Content.IsNullOrEmpty();
        }
        
        private async Task<bool> CheckState(DeveloperAgentState state)
        {
            return !state.Content.IsNullOrEmpty();
        }
    }
}