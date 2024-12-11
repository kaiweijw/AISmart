using System.Threading.Tasks;
using Orleans;

namespace AISmart.Agents;

public interface IAgent<TEvent>:  IGrainWithGuidKey
    where TEvent : class
{
    //used for activating the agent manually, mostly used for testing
    Task ActivateAsync();
    
    //probably need a function to get event description
    //Task<string> GetEventDescriptionAsync();
    
    //Function to get agent description
    Task<string> GetDescriptionAsync();

    Task AckAsync(EventWrapper<TEvent> eventWrapper);

}