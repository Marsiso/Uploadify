using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;

namespace Uploadify.Server.Domain.Infrastructure.Requests.Models;

public class BaseResponse
{
    public BaseResponse(Status status = Status.BadRequest, RequestFailure? failure = null)
    {
        Status = status;
        Failure = failure;
    }

    public BaseResponse(BaseResponse? response)
    {
        Status = response?.Status ?? Status.InternalServerError;
        Failure = response?.Failure ?? new RequestFailure
        {
            UserFriendlyMessage = Translations.RequestStatuses.InternalServerError,
            Exception = new InternalServerException()
        };
    }

    public Status Status { get; set; }
    public RequestFailure? Failure { get; set; }
}
