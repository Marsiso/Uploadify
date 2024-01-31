namespace Uploadify.Server.Domain.Requests.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string userName, string resourceID, string resourceName)
    {
        UserName = userName;
        ResourceID = resourceID;
        ResourceName = resourceName;
    }

    public string UserName { get; set; }
    public string ResourceID { get; set; }
    public string ResourceName { get; set; }
}
