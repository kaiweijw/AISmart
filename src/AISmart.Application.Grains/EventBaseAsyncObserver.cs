using AISmart.Agents;
using Orleans.Streams;

namespace AISmart.Application.Grains;

public class EventBaseAsyncObserver(Action<EventWrapperBase> action) : IAsyncObserver<EventWrapperBase>
{
    public async Task OnNextAsync(EventWrapperBase item, StreamSequenceToken? token = null)
    {
        action(item);
    }

    public Task OnCompletedAsync()
    {
        throw new NotImplementedException();
    }

    public Task OnErrorAsync(Exception ex)
    {
        throw new NotImplementedException();
    }
}