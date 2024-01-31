using System.Globalization;
using System.Security.Claims;
using CommunityToolkit.Diagnostics;
using OpenIddict.Abstractions;
using Uploadify.Authorization.Constants;
using Uploadify.Server.Domain.Application.Models;

namespace Uploadify.Server.Application.Authentication.Helpers;

public static class AuthenticationHelpers
{
    public static IEnumerable<Claim> GetClaims(User user, IEnumerable<Role> roles)
    {
        Guard.IsNotNull(user);
        return new List<Claim>
        {
            new Claim(OpenIddictConstants.Claims.Subject, user.Id),
            new Claim(OpenIddictConstants.Claims.PhoneNumber, user.PhoneNumber),
            new Claim(OpenIddictConstants.Claims.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString()),
            new Claim(OpenIddictConstants.Claims.Name, user.UserName),
            new Claim(OpenIddictConstants.Claims.GivenName, user.GivenName),
            new Claim(OpenIddictConstants.Claims.FamilyName, user.FamilyName),
            new Claim(OpenIddictConstants.Claims.Email, user.Email),
            new Claim(OpenIddictConstants.Claims.EmailVerified, user.EmailConfirmed.ToString()),
            new Claim(OpenIddictConstants.Claims.UpdatedAt, user.DateUpdated.ToString(CultureInfo.InvariantCulture)),
            new Claim(Permissions.Claims.Permission, roles.Select(role => (int)role.Permission).Aggregate(0, (l, r) => l | r).ToString())
        };
    }
}
