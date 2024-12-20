using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AISmart.Domain.OpenIddict.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Shouldly;
using Volo.Abp.Identity.Settings;
using Volo.Abp.OpenIddict.Applications;
using Xunit;

namespace Volo.Abp.OpenIddict.Authorizations;

public class PumpFunAuthorization_Tests : OpenIddictDomainTestBase
{
    private readonly IOpenIddictAuthorizationStore<OpenIddictAuthorizationModel> _authorizationStore;
    private readonly AbpOpenIddictTestData _testData;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;



    public PumpFunAuthorization_Tests()
    {
        _authorizationStore =
            ServiceProvider.GetRequiredService<IOpenIddictAuthorizationStore<OpenIddictAuthorizationModel>>();
        _testData = ServiceProvider.GetRequiredService<AbpOpenIddictTestData>();
        _applicationManager = ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        _authorizationManager = ServiceProvider.GetRequiredService<IOpenIddictAuthorizationManager>();
    }


    [Fact]
    public async Task Client_CaLL_Async()
    {
        await _applicationManager.CreateAsync(await GetOpenIddictApplicationModelAsync(_testData.PumpFunId,
            new AbpApplicationDescriptor
            {
                ClientId = _testData.PumpFunIdClientId,
                ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
                DisplayName = "Pump.Fun",
                RedirectUris =
                {
                    new Uri("https://abp.io")
                },
                PostLogoutRedirectUris =
                {
                    new Uri("https://abp.io")
                },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Device,
                    OpenIddictConstants.Permissions.Endpoints.Introspection,
                    OpenIddictConstants.Permissions.Endpoints.Revocation,
                    OpenIddictConstants.Permissions.Endpoints.Logout,

                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.Implicit,
                    OpenIddictConstants.Permissions.GrantTypes.Password,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.GrantTypes.DeviceCode,
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,

                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken,
                    OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken,
                    OpenIddictConstants.Permissions.ResponseTypes.CodeToken,
                    OpenIddictConstants.Permissions.ResponseTypes.IdToken,
                    OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken,
                    OpenIddictConstants.Permissions.ResponseTypes.None,
                    OpenIddictConstants.Permissions.ResponseTypes.Token,

                    OpenIddictConstants.Permissions.Scopes.Roles,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Address,
                    OpenIddictConstants.Permissions.Scopes.Phone,

                    OpenIddictConstants.Permissions.Prefixes.Scope + _testData.Scope1Name
                },
                ClientUri = "https://abp.io/TestApplication",
                LogoUri = "https://abp.io/TestApplication.png"
            }));

        var appPumpFunIdClient = ((await _applicationManager.FindByClientIdAsync(_testData.PumpFunIdClientId))!)
            .As<OpenIddictApplicationModel>();

        
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);


        identity.SetScopes("openid");
        
        var authorizationCreated   = await _authorizationManager.CreateAsync(
            identity: identity,
            subject : "Pump.Fun.User",
            client  : (await _applicationManager.GetIdAsync(appPumpFunIdClient))!,
            type    : OpenIddictConstants.AuthorizationTypes.Permanent,
            scopes  : identity.GetScopes());
        
        identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorizationCreated));
        
        // SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        new SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        
        var authorizationId = await _authorizationManager.GetIdAsync( authorizationCreated, CancellationToken.None);
        
        authorizationId.ShouldNotBeNull();
        
        var authorization = await _authorizationManager.FindByIdAsync( authorizationId, CancellationToken.None);

        authorization.ShouldNotBeNull();
        var subject = await _authorizationManager.GetSubjectAsync(authorizationCreated);
        subject.ShouldBe("Pump.Fun.User");
        
        var type = await _authorizationManager.GetTypeAsync(authorizationCreated);
        type.ShouldBe(OpenIddictConstants.AuthorizationTypes.Permanent);

        var status = await _authorizationManager.GetStatusAsync(authorizationCreated);
        status.ShouldBe(OpenIddictConstants.Statuses.Valid);
        
        var applicationId = await _authorizationManager.GetApplicationIdAsync(authorizationCreated);
        applicationId.ShouldBe(_testData.PumpFunId.ToString());
        
    }

    private async Task<OpenIddictApplicationModel> GetOpenIddictApplicationModelAsync(Guid id,
        OpenIddictApplicationDescriptor applicationDescriptor)
    {
        var application = new OpenIddictApplicationModel { Id = id };
        await _applicationManager.PopulateAsync(application, applicationDescriptor);
        return application;
    }
}