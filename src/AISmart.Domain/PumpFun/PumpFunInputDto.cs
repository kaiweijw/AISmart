using System.Collections.Generic;
using AISmart.Dto;
using System.Text.Json.Serialization;

namespace AISmart.PumpFun;

public class PumpFunInputDto 
{
    // TODO：需要提供一个接口，创建agent
    public string? ChatId { get; set; } 
    
    public string? AgentId { get; set; } 
    
    public string? RequestMessage { get; set; } 
    
    public string? ReplyId { get; set; } 
    
}
