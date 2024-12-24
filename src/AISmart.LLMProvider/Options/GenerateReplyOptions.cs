using AISmart.LLMProvider.Function;
using AISmart.LLMProvider.Message;
using Json.Schema;

namespace AISmart.LLMProvider.Options;

public class GenerateReplyOptions
{
    public GenerateReplyOptions()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="other">other option to copy from</param>
    public GenerateReplyOptions(GenerateReplyOptions other)
    {
        this.Temperature = other.Temperature;
        this.MaxToken = other.MaxToken;
        this.StopSequence = other.StopSequence?.Select(s => s)?.ToArray();
        this.Functions = other.Functions?.Select(f => f)?.ToArray();
        this.OutputSchema = other.OutputSchema;
    }

    public float? Temperature { get; set; }

    public int? MaxToken { get; set; }

    public string[]? StopSequence { get; set; }

    public FunctionContract[]? Functions { get; set; }

    /// <summary>
    /// Structural schema for the output. This property only applies to certain LLMs.
    /// </summary>
    public JsonSchema? OutputSchema { get; set; }
}
