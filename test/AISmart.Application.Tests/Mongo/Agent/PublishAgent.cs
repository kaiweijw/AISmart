

using System.Threading.Tasks;
using AISmart.Agents.X.Events;
using Orleans;
using Orleans.Providers;

namespace  AISmart.Mongo.Agent;

public interface ISenderAgent:IGrainWithGuidKey
{
    Task PublishEventAsync<T>(T @event);

    Task<PublishAgentState> GetState();
    
    Task<int> GetVersion();



}


[GenerateSerializer]
public class PublishAgentState
{
    [Id(0)]  public string? Content { get; set; }

    public void Apply(XThreadCreatedEvent xThreadCreatedEvent)
    {
        Content = xThreadCreatedEvent.Content + " has been applied";
    }
}



[StorageProvider(ProviderName = "PubSubStore")]
// [LogConsistencyProvider(ProviderName = "LogStorage")]
[LogConsistencyProvider(ProviderName = "CustomStorage")]
public class PublishAgent : MongoStorage<PublishAgentState, XThreadCreatedEvent>, ISenderAgent
{
    public PublishAgent() : base()
    {
    }
    

    public async Task  PublishEventAsync<T>(T @event)
    {
        base.RaiseEvent((@event as XThreadCreatedEvent));
        await base.ConfirmEvents();
    }

    public  Task<PublishAgentState> GetState()
    {
        return Task.FromResult(base.State);
    }

    public Task<int> GetVersion()
    {
        return Task.FromResult(base.Version);
    }
}