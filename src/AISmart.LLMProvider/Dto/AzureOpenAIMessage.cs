namespace AISmart.LLMProvider;

public class AzureOpenAIMessage : ILLMMessage
{
    public string content { get; set; }
    public string role { get; set; }
}