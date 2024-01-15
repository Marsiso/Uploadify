namespace Uploadify.Server.Domain.Requests.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string objectName)
    {
        ObjectName = objectName;
    }

    public string ObjectName { get; set; }
}
