using System.Net;
using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using OpenIddict.Validation.AspNetCore;
using Uploadify.Server.Application.Application.Queries;
using Uploadify.Server.Domain.Pagination.Models;
using Uploadify.Server.ResourceServer.Infrastructure.Controllers.Models;

namespace Uploadify.Server.ResourceServer.Controllers;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class RoleController : BaseApiController<UserController>
{
    public RoleController(IMediator mediator, ILogger<UserController> logger) : base(mediator, logger)
    {
    }

    [HttpGet("~/api/role/{roleID}/overview")]
    [ProducesResponseType(typeof(RoleOverviewQueryResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RoleOverviewQueryResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RoleOverviewQueryResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RoleOverviewQueryResponse), StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RoleOverviewQueryResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetRoleOverview(string? roleID, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(new RoleOverviewQuery(roleID), cancellationToken));

    [HttpGet("~/api/roles/summary")]
    [ProducesResponseType(typeof(RolesSummaryQueryResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RolesSummaryQueryResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RolesSummaryQueryResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetRolesSummary([FromQuery] RoleQueryString queryString, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(new RolesSummaryQuery(queryString), cancellationToken));
}
