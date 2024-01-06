using System.Globalization;
using System.Security.Claims;
using CommunityToolkit.Diagnostics;
using OpenIddict.Abstractions;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Infrastructure.Authorization.Constants;

namespace Uploadify.Server.Application.Authentication.Helpers;

public static class AuthenticationHelpers
{
    public static IEnumerable<Claim> GetClaimsFrom(User user)
    {
        Guard.IsNotNull(user);

        var claims = new List<Claim>
        {
            new Claim(OpenIddictConstants.Claims.Subject, user.UserName),
            new Claim(OpenIddictConstants.Claims.Name, user.UserName),
            new Claim(OpenIddictConstants.Claims.Email, user.Email),
            new Claim(OpenIddictConstants.Claims.PhoneNumber, user.PhoneNumber),
            new Claim(OpenIddictConstants.Claims.GivenName, user.GivenName),
            new Claim(OpenIddictConstants.Claims.FamilyName, user.FamilyName),
            new Claim(OpenIddictConstants.Claims.EmailVerified, user.EmailConfirmed.ToString()),
            new Claim(OpenIddictConstants.Claims.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString()),
            new Claim(OpenIddictConstants.Claims.UpdatedAt, user.DateUpdated.ToString(CultureInfo.InvariantCulture))
        };

        if (user.Roles != null)
        {
            claims.Add(new Claim(Claims.Permission, user.Roles.Select(assignment => (int)assignment.Role.Permission).Aggregate(0, (l, r) => l | r).ToString()));
            claims.AddRange(user.Roles.Select(role => new Claim(OpenIddictConstants.Claims.Role, role.Role.Name)));
        }

        return claims;
    }
}
