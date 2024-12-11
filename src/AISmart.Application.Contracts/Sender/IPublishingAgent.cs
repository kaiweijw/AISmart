using System.Threading.Tasks;
using Orleans;

namespace AISmart.Sender;

public interface IPublishingAgent : IGrainWithGuidKey
{
    Task PublishEventAsync<T>(T @event);
}