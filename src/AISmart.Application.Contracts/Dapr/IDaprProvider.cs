using System.Threading.Tasks;

namespace AISmart.Dapr;

public interface IDaprProvider
{
    Task PublishEventAsync<T>(string pubsubName, string topicName, T message);

}