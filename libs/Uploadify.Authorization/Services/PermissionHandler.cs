using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Uploadify.Authorization.Constants;
using Uploadify.Authorization.Helpers;
using Uploadify.Authorization.Models;

namespace Uploadify.Authorization.Services;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        Claim? permissionClaim = context.User.FindFirst(Permissions.Claims.Permission);
        if (permissionClaim == null)
        {
            return Task.CompletedTask;
        }

        Permission permissions = PolicyNameHelpers.GetPermissionsFrom(permissionClaim.Value);
        if ((permissions & requirement.Permission) != 0)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
