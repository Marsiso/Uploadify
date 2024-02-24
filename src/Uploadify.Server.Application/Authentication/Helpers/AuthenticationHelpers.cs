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
            new(OpenIddictConstants.Claims.Subject, user.Id),
            new(OpenIddictConstants.Claims.PhoneNumber, user.PhoneNumber),
            new(OpenIddictConstants.Claims.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString()),
            new(OpenIddictConstants.Claims.Name, user.UserName),
            new(OpenIddictConstants.Claims.GivenName, user.GivenName),
            new(OpenIddictConstants.Claims.FamilyName, user.FamilyName),
            new(OpenIddictConstants.Claims.Email, user.Email),
            new(OpenIddictConstants.Claims.EmailVerified, user.EmailConfirmed.ToString()),
            new(OpenIddictConstants.Claims.UpdatedAt, user.DateUpdated.ToString(CultureInfo.InvariantCulture)),
            new(Permissions.Claims.Permission, roles.Select(role => (int)role.Permission).Aggregate(0, (l, r) => l | r).ToString())
        };
    }
}
