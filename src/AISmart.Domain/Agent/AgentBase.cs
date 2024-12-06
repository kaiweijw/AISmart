using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using AISmart.Agent.Ability;
using AISmart.Agent.Coordinator;
using AISmart.Events;

namespace AISmart.Agent;

public abstract class AgentBase:IAgent
{
    public async Task HandleEventAsync(EventCreateEvent eventData)
    {
        Console.WriteLine("Handle EventCreateEvent");
        await HandleEventCreateEvent(eventData);
    }

    public Task HandleEventAsync(EventDispathchEvent eventData)
    {
        throw new NotImplementedException();
    }

    public Task HandleEventAsync(EventFinishEvent eventData)
    {
        throw new NotImplementedException();
    }

    public Task AddAbilityList(List<AbilityDescription> abilities)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAbilities(List<AbilityDescription> abilities)
    {
        throw new NotImplementedException();
    }

    public Task<List<AbilityDescription>> GetAllAbilityList()
    {
        throw new NotImplementedException();
    }

    public Task<Tuple<bool, string>> HasAbility(Event eventInfo)
    {
        throw new NotImplementedException();
    }

    public Task<List<AbilityParam>> ConvertEventToAbilityParam(Event eventInfo)
    {
        throw new NotImplementedException();
    }

    public Task HandleEventAsync(AbilityCreateEvent eventData)
    {
        throw new NotImplementedException();
    }

    public Task HandleEventAsync(AbilityRemoveEvent eventData)
    {
        throw new NotImplementedException();
    }

    public Task<CoordinatorResult> Coordinator(Event eventDescription, List<IAgent> agents)
    {
        throw new NotImplementedException();
    }

    public Guid Id { get; }
    public string AgentName { get; }
    public Guid ParentAgentId { get; }
    public Task<Tuple<bool, List<EventHandleResult>>> HandleEvent(Event @event, AbilityParam abilityParam)
    {
        throw new NotImplementedException();
    }
    #region need override

    protected abstract Task HandleEventCreateEvent(EventCreateEvent eventData);

    protected abstract Task HandleEventDispatchEvent(EventDispathchEvent eventData);

    #endregion
}