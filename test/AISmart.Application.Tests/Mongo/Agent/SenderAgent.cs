

using System.Threading.Tasks;
using AISmart.Agents.X.Events;
using Orleans;
using Orleans.Providers;

namespace  AISmart.Mongo.Agent;

public interface ISenderAgent:IGrainWithGuidKey
{
    Task PublishEventAsync<T>(T @event);
}


[GenerateSerializer]
public class SenderAgentState
{
    [Id(0)]  public string Content { get; set; }
}



[StorageProvider(ProviderName = "PubSubStore")]
// [LogConsistencyProvider(ProviderName = "LogStorage")]
[LogConsistencyProvider(ProviderName = "CustomStorage")]
public class SenderAgent : MongoStorage<SenderAgentState, XThreadCreatedEvent>, ISenderAgent
{
    public SenderAgent() : base()
    {
    }
    

    public Task PublishEventAsync<T>(T @event)
    {
        State.Content = (@event as XThreadCreatedEvent).Content+" commit";
        RaiseEvent((@event as XThreadCreatedEvent));
        base.ConfirmEvents();
        return Task.CompletedTask;
    }
}