using System.Threading.Tasks;
using AISmart.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AISmart.Controllers;

public class DemoController
{
    private readonly ILogger<DemoController> _logger;
    private readonly IDemoAppService _demoAppService;

    public DemoController(ILogger<DemoController> logger, IDemoAppService demoAppService)
    {
        _logger = logger;
        _demoAppService = demoAppService;
    }
    
    [HttpGet("demo")]
    public async Task<string> PostMessages(string content)
    {
        return await _demoAppService.PipelineDemoAsync(content);
    }
}