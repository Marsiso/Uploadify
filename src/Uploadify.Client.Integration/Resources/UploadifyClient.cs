namespace Uploadify.Client.Integration.Resources;

public partial class UploadifyClient;

public partial class ApiCallResponse
{
    public bool IsSuccess() => StatusCode is >= 200 and < 400;
}
