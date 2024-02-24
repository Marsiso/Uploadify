using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using Uploadify.Client.Application.Authentication.Helpers;

namespace Uploadify.Client.Api.Controllers;

public class AuthenticationController : Controller
{
    [HttpGet("~/login")]
    public ActionResult LogIn(string returnUrl)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/"
        };

        return Challenge(properties, OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpPost("~/logout")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> LogOut(string returnUrl)
    {
        var result = await HttpContext.AuthenticateAsync();
        if (result is not { Succeeded: true })
        {
            return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl : "/");
        }

        await HttpContext.SignOutAsync();

        var properties = new AuthenticationProperties(new Dictionary<string, string>
        {
            [OpenIddictClientAspNetCoreConstants.Properties.IdentityTokenHint] = result.Properties.GetTokenValue(OpenIddictClientAspNetCoreConstants.Tokens.BackchannelIdentityToken)
        })
        {
            RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/"
        };

        return SignOut(properties, OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpGet("~/callback/login/{provider}")]
    [HttpPost("~/callback/login/{provider}")]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult> LogInCallback()
    {
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

        if (result.Principal is not { Identity.IsAuthenticated: true })
        {
            throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
        }

        var identity = new ClaimsIdentity(
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role,
            authenticationType: "ExternalLogin");

        AuthenticationHelpers.MapClaims(result.Principal, identity);

        identity.SetClaim(OpenIddictConstants.Claims.Private.RegistrationId, result.Principal.GetClaim(OpenIddictConstants.Claims.Private.RegistrationId));

        var properties = new AuthenticationProperties(result.Properties.Items)
        {
            RedirectUri = result.Properties.RedirectUri ?? "/"
        };

        properties.StoreTokens(result.Properties.GetTokens().Where(token => token switch
        {
            {
                Name: OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken or OpenIddictClientAspNetCoreConstants.Tokens.BackchannelIdentityToken or OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken
            } => true,
            _ => false
        }));

        return SignIn(new(identity), properties);
    }

    [HttpGet("~/callback/logout/{provider}")]
    [HttpPost("~/callback/logout/{provider}")]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult> LogOutCallback()
    {
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        return Redirect(result.Properties.RedirectUri);
    }
}
