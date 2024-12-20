namespace AISmart.GAgent.Core;

public abstract partial class GAgentBase<TState, TEvent>
{
    private IGrainState<List<GrainId>> _subscribers = new GrainState<List<GrainId>>();

    protected async Task LoadSubscribersAsync()
    {
        await GrainStorage.ReadStateAsync("subscribers", this.GetGrainId(), _subscribers);
    }
    
    protected async Task AddSubscriberAsync(GrainId grainId)
    {
        await LoadSubscribersAsync();
        _subscribers.State.Add(grainId);
        await GrainStorage.WriteStateAsync("subscribers", this.GetGrainId(), _subscribers);
    }
}