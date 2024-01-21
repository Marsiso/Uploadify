using Microsoft.AspNetCore.Authorization;

namespace Uploadify.Authorization.Models;

public class PermissionRequirement(Permission permission) : IAuthorizationRequirement
{
    public Permission Permission { get; } = permission;
}
