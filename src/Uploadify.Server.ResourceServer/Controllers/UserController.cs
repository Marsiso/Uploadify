using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Uploadify.Server.Application.Application.Queries;
using Uploadify.Server.ResourceServer.Infrastructure.Controllers.Models;

namespace Uploadify.Server.ResourceServer.Controllers;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class UserController : BaseApiController<UserController>
{
    public UserController(IMediator mediator, ILogger<UserController> logger) : base(mediator, logger)
    {
    }

    /// <summary>
    ///     Retrieves user details. This endpoint requires the user to be authorized and uses the token from the request header to identify the user.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete, to enable the operation to be canceled.</param>
    /// <returns>Returns a GetUserQueryResponse object which includes the status of the GET request. The response varies based on the request status</returns>
    /// <remarks>
    ///     This endpoint is used for user the data retrieval and responds with appropriate HTTP status codes and JSON payloads based on the outcome of the user the data retrieval.
    ///
    ///     Sample request:
    ///
    ///         GET /api/user/cc0fe880-efc2-4c11-917e-fd9bd74557d0
    ///
    ///     Sample response:
    ///
    ///         {
    ///             "id": "cc0fe880-efc2-4c11-917e-fd9bd74557d0",
    ///             "userName": "John",
    ///             "givenName": "John",
    ///             "familyName": "Smith",
    ///             "email": "john.smith@gmail.com",
    ///             "emailConfirmed": false,
    ///             "phoneNumber": "681323123",
    ///             "phoneNumberConfirmed": false,
    ///             "picture": null,
    ///             "createdBy": "0b0bf3f6-be6d-467e-bbbf-3a06af21cb2a",
    ///             "updatedBy": "f20abe98-f716-4578-90b2-67f8233e8a25"
    ///         }
    ///
    /// </remarks>
    [HttpGet("~/api/user/{userID}/detail")]
    [ProducesResponseType(typeof(UserDetailQueryResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserDetailQueryResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserDetailQueryResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserDetailQueryResponse), StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserDetailQueryResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetUser(string? userID, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(new UserDetailQuery(userID), cancellationToken));
}
