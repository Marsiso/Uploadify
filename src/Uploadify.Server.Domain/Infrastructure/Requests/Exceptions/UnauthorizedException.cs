namespace Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string login, string resourceID, string resourceName)
    {
        Login = login;
        ResourceID = resourceID;
        ResourceName = resourceName;
    }

    public string Login { get; set; }
    public string ResourceID { get; set; }
    public string ResourceName { get; set; }
}
