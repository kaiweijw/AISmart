using Nest;

namespace AISmart.Application.Grains.Dto;

public class DeveloperAgentStateIndex
{
    [Keyword] public string Id { get; set; }
    [Keyword] public string Content { get; set; }
}