using System.IO;
using System.Threading.Tasks;
using AISmart.Authors;
using AISmart.Dapr;
using AISmart.Dto;
using AISmart.Provider;
using AISmart.Service;
using AISmart.Telegram;
using Asp.Versioning;
using Dapr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Volo.Abp;

namespace AISmart.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("OpenId")]
[Authorize(CommonConstants.OpenID)]
public class OpenIdController: AISmartController
{
    private readonly ILogger<OpenIdController> _logger;
    
    public OpenIdController(ILogger<OpenIdController> logger, 
        ITelegramService telegramService)
    {
        _logger = logger;
    }
    [HttpGet("get-one-message")]
    public Task<string> GetOneMessage()
    {
        return Task.FromResult("this openId result");
    }
}