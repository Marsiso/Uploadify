using System.Diagnostics.CodeAnalysis;

namespace Uploadify.Client.Domain.Resources.Models;

public class ResourceResponse
{
    public ResourceResponse() => ErrorMessages = Array.Empty<string>();

    public ResourceResponse(string[] errorMessages) => ErrorMessages = errorMessages;

    public bool IsSuccess => this is { ErrorMessages.Length: <= 0 };
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

    [MemberNotNullWhen(true, nameof(Resource))]
    public new bool IsSuccess => this is { Resource: not null, ErrorMessages.Length: <= 0 };

    public TResource? Resource { get; set; }
}
