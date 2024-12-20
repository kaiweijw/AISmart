using System;
using System.Threading.Tasks;
using AISmart.Dto;
using AISmart.PumpFun;
using AISmart.Service;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Volo.Abp;

namespace AISmart.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("pumpfun")]
public class PumpFunController : AISmartController
{
    private readonly ILogger<PumpFunController> _logger;
    private readonly IPumpFunChatService _pumpFunChatService;
    
    public PumpFunController(ILogger<PumpFunController> logger, IPumpFunChatService pumpFunChatService)
    {
        _logger = logger;
        _pumpFunChatService = pumpFunChatService;
    }
    
    [HttpPost("chat")]
    public Task<PumpFunOutputDto> Chat([FromBody]PumpFunInputDto inputDto)
    {
        var replyId = Guid.NewGuid().ToString();
        var headers = Request.Headers;
        var token = headers["Pump-Fun-Api-Secret-Token"];
        // TODO:jim
        _logger.LogInformation("Receive update message from telegram.{token}",token);
        _logger.LogInformation("Receive update message from telegram.{inputDto}",JsonConvert.SerializeObject(inputDto));

        inputDto.ReplyId = replyId;
        _pumpFunChatService.ReceiveMessagesAsync(inputDto);

        var outputDto = new PumpFunOutputDto
        {
            ReplyId = replyId
        };
        return Task.FromResult(outputDto);
    }
    
    [HttpPost("setGroup")]
    public async Task SetGroupsAsync(string chatId, string botName)
    {
        await _pumpFunChatService.SetGroupsAsync(chatId, botName);
    }
    
    [HttpPost("search")]
    public Task<PumFunResponseDto> SearchByReplyId(string replyId)
    {
        _logger.LogDebug("Receive message from SearchByReplyId, replyId:{replyId}", replyId);
        var response = _pumpFunChatService.SearchAnswerAsync(replyId);
        return Task.FromResult(response.Result);
    }
}