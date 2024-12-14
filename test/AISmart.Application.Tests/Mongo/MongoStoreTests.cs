using System;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.X.Events;
using AISmart.Application.Grains.Agents.X;
using AISmart.Mongo.Agent;
using AISmart.Sender;
using Orleans;
using Xunit;
using Xunit.Abstractions;

namespace AISmart.Samples
{
    public class MongoStoreTests : AISmartApplicationTestBase, IAsyncLifetime
    {
        private readonly IClusterClient _clusterClient;

        private IStateAgent<XAgentState> _xStateAgent;
        private ISenderAgent _publishingAgent;


        public MongoStoreTests(ITestOutputHelper output)
        {
            _clusterClient = GetRequiredService<IClusterClient>();
        }

        public async Task InitializeAsync()
        {
            _publishingAgent = _clusterClient.GetGrain<ISenderAgent>(Guid.NewGuid());
            _publishingAgent.PublishEventAsync(new XThreadCreatedEvent
            {
                Id = "mock_x_thread_id",
                Content = "BTC REACHED 1000k WOOHOOOO!"
            });

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

            await Task.Delay(1000 * 10);
        }
    }
}