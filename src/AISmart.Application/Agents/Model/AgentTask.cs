using System;
using System.Text.Json.Serialization;

namespace AISmart.Agents;

public class AgentTask
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Content { get; set; }
    public bool success { get; set; }
    [JsonPropertyName("created_datetime")]
    public DateTime CreatedDateTime { get; set; }
    [JsonPropertyName("updated_datetime")]
    public DateTime UpdatedDateTime { get; set; }
    [JsonPropertyName("agent_id")]
    public string AgentId { get; set; }
    [JsonPropertyName("agent_name")]
    public string AgentName { get; set; }

    public AgentTask()
    {
    }
}