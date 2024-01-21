using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Uploadify.Authorization.Helpers;
using Uploadify.Authorization.Models;
using static System.String;

namespace Uploadify.Authorization.Components;

public class PermissionView : AuthorizeView
{
    [Parameter]
    public Permission Permission
    {
        get => IsNullOrWhiteSpace(Policy) ? Permission.None : PolicyNameHelpers.GetPermissionsFrom(Policy);
        set => Policy = PolicyNameHelpers.GetPolicyNameFor(value);
    }
}
