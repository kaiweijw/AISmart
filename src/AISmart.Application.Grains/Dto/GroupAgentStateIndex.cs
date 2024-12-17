using Nest;

namespace AISmart.Application.Grains.Dto;

public class GroupAgentStateIndex
{
    [Keyword] public string Id { get; set; }
    public int RegisteredAgents { get; set; }

}