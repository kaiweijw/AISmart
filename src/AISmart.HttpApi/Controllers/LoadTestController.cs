using System.IO;
using System.Threading.Tasks;
using AISmart.Application;
using AISmart.Authors;
using AISmart.Dapr;
using AISmart.Dto;
using AISmart.Provider;
using AISmart.Service;
using AISmart.Telegram;
using Asp.Versioning;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Volo.Abp;

namespace AISmart.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("loadTest")]
public class LoadTestController: AISmartController
{
    private readonly ILogger<LoadTestController> _logger;
    private readonly IDemoAppService _demoAppService;

    public LoadTestController(ILogger<LoadTestController> logger, 
        IDemoAppService demoAppService)
    {
        _logger = logger;
        _demoAppService = demoAppService;
    }

    [HttpGet("load-test")]
    public async Task PostMessages(int aGAgentCount, int bGAgentCount, int cGAgentCount)
    {
        await _demoAppService.AgentLoadTest( aGAgentCount,  bGAgentCount,  cGAgentCount);
    }
}