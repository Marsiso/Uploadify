namespace Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string objectName)
    {
        ObjectName = objectName;
    }

    public string ObjectName { get; set; }
}
