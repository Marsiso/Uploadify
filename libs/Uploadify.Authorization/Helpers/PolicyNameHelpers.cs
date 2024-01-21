using Uploadify.Authorization.Models;
using static System.String;

namespace Uploadify.Authorization.Helpers;

public static class PolicyNameHelpers
{
    public const string Prefix = "";

    public static bool IsValidPolicyName(string? policyName) => !IsNullOrWhiteSpace(policyName) && policyName.StartsWith(Prefix);

    public static string GetPolicyNameFor(Permission permission) => $"{Prefix}{(int)permission}";

    public static Permission GetPermissionsFrom(string? policyName)
    {
        if (IsNullOrWhiteSpace(policyName))
        {
            return Permission.None;
        }

        if (int.TryParse(policyName[Prefix.Length..], out int permissionsValue))
        {
            return (Permission)permissionsValue;
        }

        return Permission.None;
    }
}
