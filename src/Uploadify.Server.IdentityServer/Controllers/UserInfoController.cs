using System.Net.Mime;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Authorization.Constants;

namespace Uploadify.Server.IdentityServer.Controllers;

public class UserInfoController : Controller
{
 private readonly UserManager<User> _manager;

    public UserInfoController(UserManager<User> manager)
    {
        _manager = manager;
    }

    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("~/connect/userinfo")]
    [HttpPost("~/connect/userinfo")]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UserInfo()
    {
        var user = await _manager.FindByIdAsync(User.GetClaim(OpenIddictConstants.Claims.Subject));
        if (user == null)
        {
            return Challenge(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The specified access token is bound to an account that no longer exists."
                }));
        }

        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [OpenIddictConstants.Claims.Subject] = await _manager.GetUserIdAsync(user)
        };

        if (User.HasScope(Scopes.Name))
        {
            claims[OpenIddictConstants.Claims.Name] = user.UserName;
            claims[OpenIddictConstants.Claims.GivenName] = user.GivenName;
            claims[OpenIddictConstants.Claims.FamilyName] = user.FamilyName;
        }

        if (User.HasScope(Scopes.Email))
        {
            claims[OpenIddictConstants.Claims.Email] = await _manager.GetEmailAsync(user);
            claims[OpenIddictConstants.Claims.EmailVerified] = await _manager.IsEmailConfirmedAsync(user);
        }

        if (User.HasScope(Scopes.Phone))
        {
            claims[OpenIddictConstants.Claims.PhoneNumber] = await _manager.GetPhoneNumberAsync(user);
            claims[OpenIddictConstants.Claims.PhoneNumberVerified] = await _manager.IsPhoneNumberConfirmedAsync(user);
        }

        if (User.HasScope(Scopes.Roles))
        {
            claims[OpenIddictConstants.Claims.Role] = await _manager.GetRolesAsync(user);
        }

        return Ok(claims);
    }
}
