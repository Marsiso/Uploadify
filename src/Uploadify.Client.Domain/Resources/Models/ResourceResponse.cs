namespace Uploadify.Client.Domain.Resources.Models;

public class ResourceResponse
{
    public ResourceResponse()
    {
        IsSuccess = true;
        ErrorMessages = Array.Empty<string>();
    }

    public ResourceResponse(string[] errorMessages)
    {
        IsSuccess = false;
        ErrorMessages = errorMessages;
    }

    public bool IsSuccess { get; set; }
    public string[] ErrorMessages { get; set; }
}

public class ResourceResponse<TResource> : ResourceResponse where TResource : class
{
    public ResourceResponse(TResource? resource)
    {
        Resource = resource;
    }

    public ResourceResponse(string[] errorMessages) : base(errorMessages)
    {
    }

    public TResource? Resource { get; set; }
}
