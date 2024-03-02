using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Uploadify.Authorization.Attributes;
using Uploadify.Authorization.Models;
using Uploadify.Server.Application.Application.Queries;
using Uploadify.Server.Domain.Infrastructure.Pagination.Models.Application;
using Uploadify.Server.ResourceServer.Infrastructure.Controllers.Models;

namespace Uploadify.Server.ResourceServer.Controllers;

public class RoleController : BaseApiController<UserController>
{
    public RoleController(IMediator mediator, ILogger<UserController> logger) : base(mediator, logger)
    {
    }

    /// <summary>
    ///     Retrieves the details of a specific role.
    /// </summary>
    /// <remarks>
    ///     This endpoint returns detailed information about a specific role identified by the roleID.
    ///     Requires 'ViewRoles' permission to access this endpoint.
    ///
    ///     Sample request:
    ///
    ///         GET: /api/roles/3/overview
    ///
    ///     Sample response:
    ///
    ///         {
    ///             "name": "system_admin",
    ///             "permission": -2,
    ///             "userCreatedBy": null,
    ///             "dateCreated": "2024-01-17T21:06:45.447448Z",
    ///             "userUpdatedBy": null,
    ///             "dateUpdated": "2024-01-28T19:09:32.250184Z"
    ///         }
    ///
    /// </remarks>
    /// <param name="roleID">The unique identifier of the role.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the action result of the role overview query.
    /// </returns>
    /// <response code="200">If the role details are successfully retrieved.</response>
    /// <response code="400">If the request is malformed or invalid.</response>
    /// <response code="401">If the user is unauthorized to perform this action.</response>
    /// <response code="404">If the specified role is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("~/api/role/{roleID}/overview")]
    [Permission(Permission.ViewRoles, AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(GetRoleOverviewQueryResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetRoleOverviewQueryResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetRoleOverviewQueryResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetRoleOverviewQueryResponse), StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetRoleOverviewQueryResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Overview(string? roleID, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(new GetRoleOverviewQuery(roleID), cancellationToken));

    /// <summary>
    ///     Retrieves a summary of all roles, supporting pagination.
    /// </summary>
    /// <remarks>
    ///     This endpoint provides a summary of all roles with pagination support.
    ///     Requires 'ViewRoles' permission to access this endpoint.
    ///
    ///     Sample request:
    ///
    ///         GET: /api/roles/summary
    ///
    ///     Sample response:
    ///
    ///         {
    ///             "summary": {
    ///                 "roles": [
    ///                     {
    ///                         "name": "user",
    ///                         "permission": 1,
    ///                         "userCreatedBy": null,
    ///                         "dateCreated": "2024-01-17T21:06:45.617462Z",
    ///                         "userUpdatedBy": null,
    ///                         "dateUpdated": "2024-01-28T19:09:32.417203Z"
    ///                     },
    ///                     {
    ///                         "name": "role_admin",
    ///                         "permission": 24,
    ///                         "userCreatedBy": null,
    ///                         "dateCreated": "2024-01-17T21:06:45.586189Z",
    ///                         "userUpdatedBy": null,
    ///                         "dateUpdated": "2024-01-28T19:09:32.39646Z"
    ///                     },
    ///                     {
    ///                         "name": "user_admin",
    ///                         "permission": 480,
    ///                         "userCreatedBy": null,
    ///                         "dateCreated": "2024-01-17T21:06:45.557188Z",
    ///                         "userUpdatedBy": null,
    ///                         "dateUpdated": "2024-01-28T19:09:32.375629Z"
    ///                     },
    ///                     {
    ///                         "name": "system_admin",
    ///                         "permission": -2,
    ///                         "userCreatedBy": null,
    ///                         "dateCreated": "2024-01-17T21:06:45.447448Z",
    ///                         "userUpdatedBy": null,
    ///                         "dateUpdated": "2024-01-28T19:09:32.250184Z"
    ///                     }
    ///                 ],
    ///                 "pageSize": 100,
    ///                 "pageNumber": 1,
    ///                 "totalPages": 1,
    ///                 "totalItems": 4,
    ///                 "hasPrevious": false,
    ///                 "hasNext": false
    ///             },
    ///             "status": 200,
    ///             "failure": {
    ///                 "errors": null,
    ///                 "userFriendlyMessage": null
    ///             }
    ///         }
    ///
    /// </remarks>
    /// <param name="queryString">The query parameters for pagination and filtering.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the action result of the roles summary query.
    /// </returns>
    /// <response code="200">If the summary of roles is successfully retrieved.</response>
    /// <response code="401">If the user is unauthorized to perform this action.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("~/api/roles/summary")]
    [Permission(Permission.ViewRoles, AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(GetRolesSummaryQueryResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetRolesSummaryQueryResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetRolesSummaryQueryResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Summary([FromQuery] RoleQueryString queryString, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(new GetRolesSummaryQuery(queryString), cancellationToken));
}
