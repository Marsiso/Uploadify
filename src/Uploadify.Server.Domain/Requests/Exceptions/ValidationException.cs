namespace Uploadify.Server.Domain.Requests.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string objectName, string[] errors)
    {
        ObjectName = objectName;
        Errors = errors;
    }

    public string ObjectName { get; set; }
    public string[] Errors { get; set; }
}
