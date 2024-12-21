namespace AISmart.GAgent.Core;

public abstract partial class GAgentBase<TState, TEvent>
{
    private const string SubscribersStateName = "Subscribers";
    private readonly IGrainState<List<GrainId>> _subscribers = new GrainState<List<GrainId>>();

    protected async Task LoadSubscribersAsync()
    {
        await GrainStorage.ReadStateAsync(SubscribersStateName, this.GetGrainId(), _subscribers);
    }
    
    protected async Task AddSubscriberAsync(GrainId grainId)
    {
        await LoadSubscribersAsync();
        _subscribers.State ??= [];
        _subscribers.State.Add(grainId);
        await GrainStorage.WriteStateAsync(SubscribersStateName, this.GetGrainId(), _subscribers);
    }
}