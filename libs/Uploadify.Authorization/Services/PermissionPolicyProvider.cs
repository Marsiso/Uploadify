using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Uploadify.Authorization.Helpers;
using Uploadify.Authorization.Models;
using static System.String;

namespace Uploadify.Authorization.Services;

public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _options;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
        _options = options.Value;
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);
        if (IsNullOrWhiteSpace(policyName) || !PolicyNameHelpers.IsValidPolicyName(policyName))
        {
            return policy;
        }

        Permission permissions = PolicyNameHelpers.GetPermissionsFrom(policyName);

        policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(permissions))
            .Build();

        _options.AddPolicy(policyName, policy);
        return policy;
    }
}
