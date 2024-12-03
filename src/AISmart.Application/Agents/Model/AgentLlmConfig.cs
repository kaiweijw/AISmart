using System.Text.Json.Serialization;

namespace AISmart.Agents;

public class AgentLlmConfig
{
    
    [JsonPropertyName("model")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Model { get; set; }
    
    /// <summary>
    /// Completion Provider
    /// </summary>
    [JsonPropertyName("provider")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Provider { get; set; }
}