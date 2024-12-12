using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;

namespace AISmart.Agents;

public interface IAgent<TState>:  IGrainWithGuidKey
{
    //used for activating the agent manually, mostly used for testing
    Task ActivateAsync();
    
    //probably need a function to get event description
    //Task<string> GetEventDescriptionAsync();
    
    //Function to get agent description
    Task<string> GetDescriptionAsync();

    Task<TState> GetStateAsync();
    Task SubscribeTo(IAgent agent);
}