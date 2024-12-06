using AISmart.Domain.Grains.Event;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace AISmart.Application.Grains.Event;

public interface IAgent :ILocalEventHandler<BasicEvent>,IGrainWithGuidKey,ITransientDependency
{
    public Task PublishAsync(BasicEvent basicEvent);

    public Task<BasicEvent> GetEventNodeAsync();

}