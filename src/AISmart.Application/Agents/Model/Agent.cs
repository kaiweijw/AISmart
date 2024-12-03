using System;

namespace AISmart.Agents;

public class Agent
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Agent Type
    /// </summary>
    // public string Type { get; set; } = AgentType.Task;
    
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    
    public AgentLlmConfig LlmConfig { get; set; } = new();


    public Agent()
    {
    }
}