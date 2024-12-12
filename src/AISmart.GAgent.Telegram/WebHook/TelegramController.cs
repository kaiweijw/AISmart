using System.Threading.Tasks;
using AISmart.Authors;
using AISmart.Dapr;
using AISmart.Dto;
using AISmart.Provider;
using Asp.Versioning;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Volo.Abp;

namespace AISmart.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("telegram")]
public class TelegramController
{
    private readonly ILogger<TelegramController> _logger;
    private readonly ITelegramProvider _telegramProvider;
    
    public TelegramController(ILogger<TelegramController> logger, 
        ITelegramProvider telegramProvider)
    {
        _logger = logger;
        _telegramProvider = telegramProvider;
    }
    [HttpPost("messages")]
    public async Task PostMessages([FromBody]TelegramUpdateDto updateMessage)
    {
        _logger.LogDebug("Receive update message from telegram.{message}",JsonConvert.SerializeObject(updateMessage));
        await _telegramProvider.GetUpdatesMessagesAsync(updateMessage);
    }
}