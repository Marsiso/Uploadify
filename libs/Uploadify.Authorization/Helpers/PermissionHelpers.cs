using Uploadify.Authorization.Models;

namespace Uploadify.Authorization.Helpers;

public static class PermissionHelpers
{
    public static IEnumerable<Permission> GetValues() => Enum.GetValues(typeof(Permission)).OfType<Permission>();
    public static IEnumerable<Permission> GetDistinctValues(Permission? permission)
    {
        if (!permission.HasValue)
        {
            return GetValues();
        }

        return GetValues().Where(value => !permission.Value.HasFlag(value));
    }
}
