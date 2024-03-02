using System.Runtime.CompilerServices;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;

namespace Uploadify.Server.ResourceServer.Infrastructure.Controllers.Models;

[ApiController]
public class BaseApiController<TController> : ControllerBase where TController : class
{
    protected readonly IMediator Mediator;
    protected readonly ILogger<TController> Logger;

    public BaseApiController(IMediator mediator, ILogger<TController> logger)
    {
        Mediator = mediator;
        Logger = logger;
    }

    protected IActionResult ConvertToActionResult<TResponse>(TResponse response, [CallerMemberName] string? action = null) where TResponse : BaseResponse
    {
        try
        {
            return response.Status switch
            {
                Status.Ok => Ok(response),
                Status.NotFound => NotFound(response),
                Status.BadRequest => BadRequest(response),
                Status.InternalServerError => HandleError(response, action),
                _ => StatusCode((int)response.Status, response)
            };
        }
        catch (Exception exception)
        {
            Logger.LogError($"Controller: '{nameof(TController)}' Action: '{action}' Message: '{response.Failure?.UserFriendlyMessage}' Exception: '{exception}'.");
            return StatusCode((int)Status.InternalServerError, new BaseResponse(Status.InternalServerError, new() { UserFriendlyMessage = Translations.RequestStatuses.InternalServerError }));
        }
    }

    protected IActionResult HandleError<TResponse>(TResponse response, string? action) where TResponse : BaseResponse
    {
        Logger.LogError($"Controller: '{nameof(TController)}' Action: '{action}' Message: '{response.Failure?.UserFriendlyMessage}' Exception: '{response.Failure?.Exception}'.");
        return StatusCode((int)response.Status, response);
    }
}
