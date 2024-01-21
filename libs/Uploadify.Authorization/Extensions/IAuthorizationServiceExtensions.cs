using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Uploadify.Authorization.Helpers;
using Uploadify.Authorization.Models;

namespace Uploadify.Authorization.Extensions;

public static class IAuthorizationServiceExtensions
{
    public static Task<AuthorizationResult> AuthorizeAsync(this IAuthorizationService service, ClaimsPrincipal user, Permission permission) => service.AuthorizeAsync(user, user, PolicyNameHelpers.GetPolicyNameFor(permission));
}
