using System.Diagnostics;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace AISmart.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("OpenId")]
[Route("api")]
// [Authorize(CommonConstants.OpenID)]
public class OpenIdController: AISmartController
{
    private readonly ILogger<OpenIdController> _logger;
    
    public OpenIdController(ILogger<OpenIdController> logger)
    {
        _logger = logger;
    }
    [HttpGet]
    [Route("get-one-message")]
    [Authorize]
    public Task<string> GetOneMessage()
    {
        Debug.Assert(User != null, nameof(User) + " != null");
        _logger.LogInformation("{userIdentityName} is calling GetOneMessage method ",User.Identity?.Name);
        return Task.FromResult("this openId result");
    }
}