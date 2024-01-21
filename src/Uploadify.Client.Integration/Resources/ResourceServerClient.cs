namespace Uploadify.Client.Integration.Resources;

public partial class ResourceServerClient;

public partial class ApiCallResponse
{
    public bool IsSuccess() => StatusCode is >= 200 and < 400;
}
