using System.Threading.Tasks;
using AISmart.Agents;
using Orleans;

namespace AISmart.Sender;

public interface IPublishingAgent : IGrainWithGuidKey
{
    Task PublishEventAsync<T>(T @event) where T : GEvent;
}