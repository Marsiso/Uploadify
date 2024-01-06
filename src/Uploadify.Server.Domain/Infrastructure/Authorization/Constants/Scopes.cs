namespace Uploadify.Server.Domain.Infrastructure.Authorization.Constants;

public static class Scopes
{
    public const string Name = "profile.name";
    public const string Email = "profile.email";
    public const string Phone = "profile.phone";
    public const string Roles = "profile.roles";
    public const string Api = "api";

    public static class Resources
    {
        public const string Write = "resource.api.write";
        public const string Read = "resource.api.read";
    }
}
