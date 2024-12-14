using System;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.X.Events;
using AISmart.Application.Grains.Agents.X;
using AISmart.Mongo.Agent;
using AISmart.Sender;
using MongoDB.Driver;
using Orleans;
using Orleans.EventSourcing;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AISmart.Samples
{
    public class MongoStoreTests : AISmartApplicationTestBase, IAsyncLifetime
    {
        private readonly IClusterClient _clusterClient;

        private ISenderAgent _publishingAgent;
        private IMongoCollection<MongoStateWrapper<PublishAgentState>> _stateCollection;



        public MongoStoreTests(ITestOutputHelper output)
        {
            _clusterClient = GetRequiredService<IClusterClient>();
        }

        public async Task InitializeAsync()
        {
            
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "AISmart";
            string stateCollectionName = "state";
            string eventCollectionName = "event";
        
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
    
            _stateCollection = database.GetCollection<MongoStateWrapper<PublishAgentState>>(stateCollectionName);
            
            _publishingAgent = _clusterClient.GetGrain<ISenderAgent>(Guid.NewGuid());
            _publishingAgent.GetVersion();
            // _publishingAgent.PublishEventAsync(new XThreadCreatedEvent
            // {
            //     Id = "mock_x_thread_id",
            //     Content = "BTC REACHED 1000k WOOHOOOO!"
            // });



        }

        public Task DisposeAsync()
        {
            // Clean up resources if needed
            return Task.CompletedTask;
        }

        [Fact]
        public async Task MongoStorage_Test()
        {
            
            var mongoStateWrapper = await _stateCollection.Find(FilterDefinition<MongoStateWrapper<PublishAgentState>>.Empty)
                .SortByDescending(s => s.Version)
                .FirstOrDefaultAsync();

            int mongoVersion = mongoStateWrapper.Version;
            PublishAgentState mongoPublishAgentState = mongoStateWrapper.State;
            
            int grainVersion = await _publishingAgent.GetVersion();
            PublishAgentState grainState = await _publishingAgent.GetState();
            
            grainVersion.ShouldBeEquivalentTo(mongoVersion);
            grainState.Content.ShouldBe(mongoPublishAgentState.Content);
            

            const string content = "BTC REACHED 100k WOOHOOOO!";
            var xThreadCreatedEvent = new XThreadCreatedEvent
            {
                Id = "mock_x_thread_id",
                Content = content
            };
            await _publishingAgent.PublishEventAsync(xThreadCreatedEvent);
            
            
            
            int updatedGrainVersion = await _publishingAgent.GetVersion();
            PublishAgentState updatedGrainState = await _publishingAgent.GetState();
            
            updatedGrainVersion.ShouldBeEquivalentTo(grainVersion+1);
            updatedGrainState.Content.ShouldBe(content + " has been applied");
           
            
            
            var updatedMongoStateWrapper = await _stateCollection.Find(FilterDefinition<MongoStateWrapper<PublishAgentState>>.Empty)
                .SortByDescending(s => s.Version)
                .FirstOrDefaultAsync();

            int updateMongoVersion = updatedMongoStateWrapper.Version;
            PublishAgentState updatedMongoState = updatedMongoStateWrapper.State;
            
            updateMongoVersion.ShouldBeEquivalentTo(mongoVersion+1);
            updatedMongoState.Content.ShouldBe(content + " has been applied");
        }
    }
}