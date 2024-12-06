using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AISmart.Agent.Ability;
using AISmart.Events;

namespace AISmart.AgentCommunicationComponent;

public interface IAgentSolutionComponent
{
    Task AddAbilityList(List<AbilityDescription> abilities);

    Task RemoveAbilities(List<AbilityDescription> abilities);

    Task<List<AbilityDescription>> GetAllAbilityList();

    Task<Tuple<bool, string>> HasAbility(Event eventInfo);

    Task<List<AbilityParam>> ConvertEventToAbilityParam(Event eventInfo);
}