
using AISmart.Agents;

namespace AISmart.Application.Grains.Agents.Developer;

public interface IDeveloperAgent<ImplementationEvent>:IAgent<ImplementationEvent> where ImplementationEvent : class
{
    
}