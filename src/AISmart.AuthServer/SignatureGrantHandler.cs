using System.Collections.Immutable;
using AElf;
using AElf.Types;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Identity;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.ExtensionGrantTypes;

namespace AISmart.AuthServer;

public class SignatureGrantHandler: ITokenExtensionGrant
{
    private ILogger<SignatureGrantHandler> _logger;
    private readonly string _source = "AISmart";
    private readonly string _V2 = "v2";

    public string Name { get; } = "signature";

    public async Task<IActionResult> HandleAsync(ExtensionGrantContext context)
    {
        var publicKeyVal = context.Request.GetParameter("pubkey").ToString();
        var signatureVal = context.Request.GetParameter("signature").ToString();
        var timestampVal = context.Request.GetParameter("timestamp").ToString();
        var inviteFrom = context.Request.GetParameter("invite_from").ToString();
        var inviteType = context.Request.GetParameter("invite_type").ToString();
        var nickName = context.Request.GetParameter("nick_name").ToString();
        
        var accountInfo = context.Request.GetParameter("accountInfo").ToString();
        var source = context.Request.GetParameter("source").ToString();
        var signTip = context.Request.GetParameter("signTip").ToString();

        var invalidParamResult = CheckParams(publicKeyVal, signatureVal, timestampVal, accountInfo, source);
        if (invalidParamResult != null)
        {
            return invalidParamResult;
        }

        var publicKey = ByteArrayHelper.HexStringToByteArray(publicKeyVal);
        var signature = ByteArrayHelper.HexStringToByteArray(signatureVal);
        var timestamp = long.Parse(timestampVal);
        var address = string.Empty;
        if (!string.IsNullOrWhiteSpace(publicKeyVal))
        {
            address = Address.FromPublicKey(publicKey).ToBase58();
        }

        var caHash = string.Empty;
        var caAddressMain = string.Empty;
        var caAddressSide = new Dictionary<string, string>();
        
        var time = DateTime.UnixEpoch.AddMilliseconds(timestamp);
        var timeRangeConfig = context.HttpContext.RequestServices.GetRequiredService<IOptionsSnapshot<TimeRangeOption>>()
            .Value;

        if (time < DateTime.UtcNow.AddMinutes(-timeRangeConfig.TimeRange) ||
            time > DateTime.UtcNow.AddMinutes(timeRangeConfig.TimeRange))
        {
            return GetForbidResult(OpenIddictConstants.Errors.InvalidRequest,
                $"The time should be {timeRangeConfig.TimeRange} minutes before and after the current time.");
        }
        _logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<SignatureGrantHandler>>();
        var userManager = context.HttpContext.RequestServices.GetRequiredService<IdentityUserManager>();

        var userName = address;
        if (!string.IsNullOrWhiteSpace(caHash))
        {
            userName = caHash;
        }
        
        var user = await userManager.FindByNameAsync(userName);
        _logger.LogInformation("miniapp user login:loginUser:{userName},address:{address}, inviteFrom{A},inviteType:{B},nickName:{C}, userDto:{user}", 
            userName,address,inviteFrom,inviteType,nickName,JsonConvert.SerializeObject(user));

        if (user == null)
        {
            _logger.LogInformation("miniapp user login:loginUser:{user} is not exist", userName);

            var userId = Guid.NewGuid();

            var createUserResult = await CreateUserAsync(userManager, userId, address, caHash,caAddressMain,caAddressSide);
            if (!createUserResult)
            {
                return GetForbidResult(OpenIddictConstants.Errors.ServerError, "Create user failed.");
            }

            user = await userManager.GetByIdAsync(userId);
        }
        else
        {
            _logger.LogInformation("miniapp user login:loginUser:{user} is exist", userName);

        }
        var signInManager = context.HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Identity.SignInManager<IdentityUser>>();
        var principal = await signInManager.CreateUserPrincipalAsync(user);
        principal.SetScopes("AISmartAuthServer");
        principal.SetResources(await GetResourcesAsync(context, principal.GetScopes()));
        principal.SetAudiences("AISmartAuthServer");
        
        var abpOpenIddictClaimDestinationsManager = context.HttpContext.RequestServices
            .GetRequiredService<AbpOpenIddictClaimsPrincipalManager>();

        await abpOpenIddictClaimDestinationsManager.HandleAsync(context.Request, principal);
        return new SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, principal);
    }
    
    private ForbidResult GetForbidResult(string errorType, string errorDescription)
    {
        return new ForbidResult(
            new[] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme },
            properties: new AuthenticationProperties(new Dictionary<string, string>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = errorType,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = errorDescription
            }!));
    }
    
    private ForbidResult? CheckParams(string publicKeyVal, string signatureVal, string timestampVal, string accoutInfo,
        string source)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(source))
        {
            errors.Add("invalid parameter source.");
        }

        if (source != _source && string.IsNullOrWhiteSpace(publicKeyVal))
        {
            errors.Add("invalid parameter pubkey.");
        }

        if (string.IsNullOrWhiteSpace(signatureVal))
        {
            errors.Add("invalid parameter signature.");
        }

        if (source != _source && string.IsNullOrWhiteSpace(timestampVal) || !long.TryParse(timestampVal, out var time) || time <= 0)
        {
            errors.Add("invalid parameter timestamp.");
        }

        if (source == _source && string.IsNullOrWhiteSpace(accoutInfo))
        {
            errors.Add("invalid parameter account_info.");
        }

        if (errors.Count > 0)
        {
            return new ForbidResult(
                new[] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme },
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidRequest,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = GetErrorMessage(errors)
                }!));
        }

        return null;
    }
    
    private static string GetErrorMessage(List<string> errors)
    {
        var message = string.Empty;

        errors?.ForEach(t => message += $"{t}, ");
        if (message.Contains(','))
        {
            return message.TrimEnd().TrimEnd(',');
        }

        return message;
    }

    private async Task<bool> CreateUserAsync(IdentityUserManager userManager, Guid userId, string address,
        string caHash,string caAddressMain,Dictionary<string, string> caAddressSide)
    {
        var result = false;
        string userName = string.IsNullOrEmpty(caHash) ? address : caHash;
        var user = new IdentityUser(userId, userName: userName, email: Guid.NewGuid().ToString("N") + "@nft-market.io");
        var identityResult = await userManager.CreateAsync(user);

        if (identityResult.Succeeded)
        {
            _logger.LogDebug($"create user success: {userId.ToString()}");
        }

        result = identityResult.Succeeded;

        return result;
    }

    private static async Task<IEnumerable<string>> GetResourcesAsync(ExtensionGrantContext context,
        ImmutableArray<string> scopes)
    {
        var resources = new List<string>();
        if (!scopes.Any())
        {
            return resources;
        }

        await foreach (var resource in context.HttpContext.RequestServices.GetRequiredService<IOpenIddictScopeManager>()
                           .ListResourcesAsync(scopes))
        {
            resources.Add(resource);
        }

        return resources;
    }
}