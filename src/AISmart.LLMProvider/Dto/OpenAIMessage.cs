namespace AISmart.LLMProvider;

public class OpenAIMessage : ILLMMessage
{
    public string content { get; set; }
    public string role { get; set; }
}