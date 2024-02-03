namespace Uploadify.Client.Domain.Routing.Constants;

public static class PageRoutes
{
    public const string Homepage = "/";
    public const string About = "/about";
    public const string Privacy = "/privacy";
    public const string Management = "/management";
    public const string More = "/more";
    public const string Profile = "/profile";
    public const string Dashboard = Homepage;
    public const string Detail = "/files-detail/{fileID:int?}";
    public const string Shared = "/shared";
    public const string PermissionAssign = "/management-permissions-assign";
}
