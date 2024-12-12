using System.Threading.Tasks;
using AISmart.Authors;
using AISmart.Dapr;
using AISmart.Dto;
using Asp.Versioning;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace AISmart.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("telegram")]
public class TelegramController
{
    private readonly ILogger<TelegramController> _logger;
    public TelegramController(ILogger<TelegramController> logger)
    {
        _logger = logger;
    }
    [HttpPost("messages")]
    public Task<long> PostMessages(TelegramUpdateDto updates)
    {
        return Task.FromResult(updates.UpdateId);
    }
}