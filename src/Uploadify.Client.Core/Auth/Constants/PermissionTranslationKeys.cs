using System.Collections.Frozen;
using Uploadify.Authorization.Models;
using static Uploadify.Authorization.Models.Permission;
using static Uploadify.Client.Domain.Localization.Constants.Translations.Authorization;

namespace Uploadify.Client.Core.Auth.Constants;

public static class PermissionTranslationKeys
{
    public static readonly FrozenDictionary<Permission, string> Values = new Dictionary<Permission, string>
    {
        [None] = Permissions.None,
        [ViewPermissions] = Permissions.ViewPermissions,
        [EditPermissions] = Permissions.EditPermissions,
        [ViewRoles] = Permissions.ViewRoles,
        [EditRoles] = Permissions.EditRoles,
        [ViewUsers] = Permissions.ViewUsers,
        [EditUsers] = Permissions.EditUsers,
        [ViewFiles] = Permissions.ViewFiles,
        [EditFiles] = Permissions.EditFiles,
        [All] = Permissions.All
    }.ToFrozenDictionary();
}
