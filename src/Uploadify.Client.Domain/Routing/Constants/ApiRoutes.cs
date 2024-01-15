namespace Uploadify.Client.Domain.Routing.Constants;

public static class ApiRoutes
{
    public const string Base = "api";

    public static class Account
    {
        public const string LogIn = "/login";
        public const string LogOut = "/logout";
    }

    public static class UserInfo
    {
        public const string BaseUserInfo = $"{Base}/userinfo";
    }
}
