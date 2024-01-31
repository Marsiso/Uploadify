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
    ///     Retrieves the details of a specific user.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns detailed information about a specific user identified by the userID.
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
    /// <param name="userID">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the action result of the user detail query.
    /// </returns>
    /// <response code="200">If the user details are successfully retrieved.</response>
    /// <response code="400">If the request is malformed or invalid.</response>
    /// <response code="401">If the user is unauthorized to perform this action.</response>
    /// <response code="404">If the specified user is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("~/api/user/{userID}/detail")]
    [ProducesResponseType(typeof(UserDetailQueryResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserDetailQueryResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserDetailQueryResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserDetailQueryResponse), StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserDetailQueryResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetUser(string? userID, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(new UserDetailQuery(userID), cancellationToken));
}
