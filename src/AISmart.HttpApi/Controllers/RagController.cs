using System.Threading.Tasks;
using AISmart.Authors;
using AISmart.Dapr;
using AISmart.Rag;
using AISmart.Rag.dto;
using Asp.Versioning;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace AISmart.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("rag")]
public class RagController
{
    private readonly ILogger<DaprController> _logger;
    private readonly IRagProvider _ragProvider;
    
    public RagController(ILogger<DaprController> logger, IRagProvider ragProvider)
    {
        _logger = logger;
        _ragProvider = ragProvider;
    }
    
    [HttpGet("token")]
    [Topic(CommonConstants.PubSubName, CommonConstants.TestGroup)]
    public Task<string> GetToken( AuthorDto authorDto)
    {
        _logger.LogInformation("GetToken {Name}",authorDto.Name);
        return Task.FromResult(authorDto.Name);
    }
    
    [HttpPost("add")]
    public Task Add( AddTextInput input)
    {
        _logger.LogInformation("add {text}", input.Text);
        return _ragProvider.StoreTextAsync(input.Text);
    }
    
    [HttpGet("retrieve")]
    public Task<string> Retrieve( RetrieveInput input)
    {
        _logger.LogInformation("retrieve {text}", input.Query);
        return _ragProvider.RetrieveAnswerAsync(input.Query);
    }
    
}