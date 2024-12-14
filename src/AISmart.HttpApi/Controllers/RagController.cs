using System.Threading.Tasks;
using AISmart.Options;
using AISmart.Provider;
using AISmart.Rag.dto;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;

namespace AISmart.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("rag")]
public class RagController
{
    private readonly IRagProvider _ragProvider;
    
    public RagController(ILogger<RagProvider> logger, IOptionsMonitor<RagOptions> ragOptions)
    {
        _ragProvider = new RagProvider(ragOptions, logger);
    }
    
    [HttpPost("add")]
    public Task Add( AddTextInput input)
    {
        return _ragProvider.StoreTextAsync(input.Text);
    }
    
    [HttpGet("retrieve")]
    public Task<string> Retrieve( RetrieveInput input)
    {
        return _ragProvider.RetrieveAnswerAsync(input.Query);
    }
    
}