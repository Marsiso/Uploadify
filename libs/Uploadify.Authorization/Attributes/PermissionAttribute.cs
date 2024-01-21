using Microsoft.AspNetCore.Authorization;
using Uploadify.Authorization.Helpers;
using Uploadify.Authorization.Models;
using static System.String;

namespace Uploadify.Authorization.Attributes;

public class PermissionAttribute : AuthorizeAttribute
{
    public PermissionAttribute()
    {
    }

    public PermissionAttribute(string policy) : base(policy)
    {
    }

    public PermissionAttribute(Permission permission) => Permission = permission;

    public Permission Permission
    {
        get => !IsNullOrWhiteSpace(Policy) ? PolicyNameHelpers.GetPermissionsFrom(Policy) : Permission.None;
        set => Policy = value != Permission.None ? PolicyNameHelpers.GetPolicyNameFor(value) : Empty;
    }
}
