using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace AISmart.Agents;

public interface IGAgent : IGrainWithGuidKey
{
    /// <summary>
    /// Used for activating the agent manually, mostly used for testing
    /// </summary>
    /// <returns></returns>
    Task ActivateAsync();

    //probably need a function to get event description
    //Task<string> GetEventDescriptionAsync();

    //Function to get agent description
    Task<string> GetDescriptionAsync();
    Task<bool> SubscribeToAsync(IGAgent agent);
    Task<bool> UnsubscribeFromAsync(IGAgent agent);
    Task<bool> PublishToAsync(IGAgent anotherGAgent);
    Task<bool> UnpublishFromAsync(IGAgent agent);
    Task RegisterAsync(IGAgent agent);
    Task UnregisterAsync(IGAgent agent);
    Task<List<Type>?> GetAllSubscribedEventsAsync();
}

public interface IStateGAgent<TState> : IGAgent
{
    Task<TState> GetStateAsync();
}