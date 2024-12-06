using AISmart.Agent.Ability;
using Volo.Abp.EventBus;

namespace AISmart.AgentCommunicationComponent;

public interface IAgentSolutionCommunicationComponent:ILocalEventHandler<AbilityCreateEvent>, ILocalEventHandler<AbilityRemoveEvent>
{
    
}