using System.Security.Claims;
using OpenIddict.Abstractions;
using Uploadify.Authorization.Constants;
using Uploadify.Client.Domain.Authentication.Models;

namespace Uploadify.Client.Application.Authentication.Helpers;

public static class AuthenticationHelpers
{
    public static UserInfo ExtractUserInfo(ClaimsPrincipal principal)
    {
        if (principal is not { Identity.IsAuthenticated: true })
        {
            return UserInfo.Anonymous;
        }

        var userInfo = new UserInfo
        {
            IsAuthenticated = true
        };

        if (principal.Identity is ClaimsIdentity identity)
        {
            userInfo.NameClaimType = identity.NameClaimType;
            userInfo.RoleClaimType = identity.RoleClaimType;
        }
        else
        {
            userInfo.NameClaimType = OpenIddictConstants.Claims.Name;
            userInfo.RoleClaimType = OpenIddictConstants.Claims.Role;
        }

        if (!principal.Claims.Any())
        {
            return userInfo;
        }

        var claims = new List<ClaimValue>();
        foreach (var claim in principal.Claims)
        {
            switch (claim.Type)
            {
                case OpenIddictConstants.Claims.Subject:
                case OpenIddictConstants.Claims.Name:
                case OpenIddictConstants.Claims.GivenName:
                case OpenIddictConstants.Claims.FamilyName:
                case OpenIddictConstants.Claims.Email:
                case OpenIddictConstants.Claims.EmailVerified:
                case OpenIddictConstants.Claims.PhoneNumber:
                case OpenIddictConstants.Claims.PhoneNumberVerified:
                case Permissions.Claims.Permission:
                    claims.Add(new ClaimValue(claim.Type, claim.Value));
                    continue;
            }
        }

        userInfo.Claims = claims;

        return userInfo;
    }

    public static void MapClaims(ClaimsPrincipal? principal, ClaimsIdentity? identity)
    {
        if (principal == null || identity == null)
        {
            return;
        }

        identity.SetClaim(OpenIddictConstants.Claims.Subject, principal.GetClaim(OpenIddictConstants.Claims.Subject));
        identity.SetClaim(OpenIddictConstants.Claims.Email, principal.GetClaim(OpenIddictConstants.Claims.Email));
        identity.SetClaim(OpenIddictConstants.Claims.EmailVerified, principal.GetClaim(OpenIddictConstants.Claims.EmailVerified));
        identity.SetClaim(OpenIddictConstants.Claims.PhoneNumber, principal.GetClaim(OpenIddictConstants.Claims.PhoneNumber));
        identity.SetClaim(OpenIddictConstants.Claims.PhoneNumberVerified, principal.GetClaim(OpenIddictConstants.Claims.PhoneNumberVerified));
        identity.SetClaim(OpenIddictConstants.Claims.Name, principal.GetClaim(OpenIddictConstants.Claims.Name));
        identity.SetClaim(OpenIddictConstants.Claims.GivenName, principal.GetClaim(OpenIddictConstants.Claims.GivenName));
        identity.SetClaim(OpenIddictConstants.Claims.FamilyName, principal.GetClaim(OpenIddictConstants.Claims.FamilyName));
        identity.SetClaim(Permissions.Claims.Permission, principal.GetClaim(Permissions.Claims.Permission));
    }
}
