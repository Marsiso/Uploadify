using Uploadify.Authorization.Models;

namespace Uploadify.Authorization.Helpers;

public static class PermissionHelpers
{
    public static List<Permission> GetValues() => Enum.GetValues(typeof(Permission)).OfType<Permission>().ToList();
}
