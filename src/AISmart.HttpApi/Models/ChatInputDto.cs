namespace AISmart.Rag.dto;

public class ChatInputDto
{
    public string callerId { get; set; }
    public string agentId { get; set; }
    public string userAddress { get; set; }
    public string RequestMessageId { get; set; }
    public string RequestMessage { get; set; }
}