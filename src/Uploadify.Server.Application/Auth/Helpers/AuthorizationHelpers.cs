using System.Security.Claims;
using OpenIddict.Abstractions;
using Uploadify.Authorization.Constants;
using Uploadify.Server.Domain.Auth.Constants;

namespace Uploadify.Server.Application.Auth.Helpers;

public static class AuthorizationHelpers
{
    public static IEnumerable<string> GetDestinations(Claim claim)
    {
        switch (claim.Type)
        {
            case OpenIddictConstants.Claims.Subject:
                yield return OpenIddictConstants.Destinations.AccessToken;
                yield return OpenIddictConstants.Destinations.IdentityToken;

                yield break;

            case OpenIddictConstants.Claims.Name:
            case OpenIddictConstants.Claims.GivenName:
            case OpenIddictConstants.Claims.FamilyName:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (claim.Subject.HasScope(Scopes.Name))
                {
                    yield return OpenIddictConstants.Destinations.IdentityToken;
                }

                yield break;

            case OpenIddictConstants.Claims.Email:
            case OpenIddictConstants.Claims.EmailVerified:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (claim.Subject.HasScope(Scopes.Email))
                {
                    yield return OpenIddictConstants.Destinations.IdentityToken;
                }

                yield break;

            case OpenIddictConstants.Claims.PhoneNumber:
            case OpenIddictConstants.Claims.PhoneNumberVerified:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (claim.Subject.HasScope(Scopes.Phone))
                {
                    yield return OpenIddictConstants.Destinations.IdentityToken;
                }

                yield break;

            case OpenIddictConstants.Claims.Role:
            case Permissions.Claims.Permission:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (claim.Subject.HasScope(Scopes.Roles))
                {
                    yield return OpenIddictConstants.Destinations.IdentityToken;
                }

                yield break;

            case Claims.SecurityStamp: yield break;

            default:
                yield return OpenIddictConstants.Destinations.AccessToken;
                yield break;
        }
    }
}
