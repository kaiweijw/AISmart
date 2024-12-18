using System.Threading.Tasks;
using AISmart.Dto;
using AISmart.Provider;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace AISmart.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("chat")]
public class ChatController
{
    private readonly IChatProvider _chatProvider;
    
    [HttpPost("chat")]
    public Task<AskOutputDto> Chat(AskInputDto inputDto)
    {
        var response = _chatProvider.AskAsync(inputDto);
        return Task.FromResult(response.Result);
    }
    
    [HttpPost("search")]
    public Task<SearchAnswerOutputDto> SearchByReplyId(string replyId)
    {
        var response = _chatProvider.SearchAnswerAsync(replyId);
        return Task.FromResult(response.Result);
    }
}