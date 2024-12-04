using System.Threading.Tasks;
using AISmart.Dapr;
using Orleans;
using Volo.Abp.Application.Services;

namespace AISmart.Mock;

public class MockDaprProvider :  IDaprProvider
{
    public MockDaprProvider()
    {
       
    }

    public  Task PublishEventAsync<T>(string pubsubName, string topicName, T message)
    {
       return Task.CompletedTask;
    }
}