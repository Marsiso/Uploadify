using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Uploadify.Authorization.Attributes;
using Uploadify.Authorization.Models;
using Uploadify.Server.Application.Auth.Commands;
using Uploadify.Server.ResourceServer.Infrastructure.Controllers.Models;

namespace Uploadify.Server.ResourceServer.Controllers;

public class PermissionController : BaseApiController<PermissionController>
{
    public PermissionController(IMediator mediator, ILogger<PermissionController> logger) : base(mediator, logger)
    {
    }

    /// <summary>
    ///     Grants specified permissions to a user.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows granting of permissions to users if the caller has the necessary rights.
    ///     Requires 'EditPermissions' permission to access this endpoint.
    ///
    ///     Sample request:
    ///
    ///         PUT: /api/permissions/grant
    ///         {
    ///             name: "user_admin",
    ///             permission: 128
    ///         }
    ///
    /// </remarks>
    /// <param name="command">The command object containing the details of the permission grant request.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the action result of the permission granting operation.
    /// </returns>
    /// <response code="200">If the permission is successfully granted.</response>
    /// <response code="400">If the request is malformed or invalid.</response>
    /// <response code="401">If the user is unauthorized to perform this action.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPut("~/api/permission/grant")]
    [IgnoreAntiforgeryToken]
    [Permission(Permission.EditPermissions, AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(GrantPermissionCommandResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GrantPermissionCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GrantPermissionCommandResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GrantPermissionCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GrantPermission([FromBody] GrantPermissionCommand command, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(command, cancellationToken));

    /// <summary>
    /// Revokes specified permissions from a user.
    /// </summary>
    /// <remarks>
    ///     This endpoint allows revoking of permissions from users if the caller has the necessary rights.
    ///     Requires 'EditPermissions' permission to access this endpoint.
    ///
    ///     Sample request:
    ///
    ///         PUT: /api/permissions/revoke
    ///         {
    ///             name: "user_admin",
    ///             permission: 128
    ///         }
    ///
    /// </remarks>
    /// <param name="command">The command object containing the details of the permission revoke request.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the action result of the permission revoking operation.
    /// </returns>
    /// <response code="200">If the permission is successfully revoked.</response>
    /// <response code="400">If the request is malformed or invalid.</response>
    /// <response code="401">If the user is unauthorized to perform this action.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPut("~/api/permission/revoke")]
    [IgnoreAntiforgeryToken]
    [Permission(Permission.EditPermissions, AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(RevokePermissionCommandResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RevokePermissionCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RevokePermissionCommandResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RevokePermissionCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> RevokePermission([FromBody] RevokePermissionCommand command, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send<RevokePermissionCommandResponse>(command, cancellationToken));
}
