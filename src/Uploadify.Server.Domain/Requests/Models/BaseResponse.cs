namespace Uploadify.Server.Domain.Requests.Models;

public class BaseResponse
{
    public BaseResponse()
    {
        Status = Status.BadRequest;
        Failure = new RequestFailure
        {
            Errors = null,
            UserFriendlyMessage = null,
            Exception = null
        };
    }

    public BaseResponse(Status status, RequestFailure? failure)
    {
        Status = status;
        Failure = failure;
    }

    public Status Status { get; set; }
    public RequestFailure? Failure { get; set; }
}
