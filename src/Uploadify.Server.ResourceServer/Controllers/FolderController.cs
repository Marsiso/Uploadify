using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Uploadify.Authorization.Attributes;
using Uploadify.Server.Application.FileSystem.Commands;
using Uploadify.Server.Application.FileSystem.Queries;
using Uploadify.Server.ResourceServer.Infrastructure.Controllers.Models;

namespace Uploadify.Server.ResourceServer.Controllers;

public class FolderController : BaseApiController<FolderController>
{
    public FolderController(IMediator mediator, ILogger<FolderController> logger) : base(mediator, logger)
    {
    }

    [HttpGet("~/api/folders/summary")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(GetFolderSummaryQueryResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderSummaryQueryResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderSummaryQueryResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderSummaryQueryResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderSummaryQueryResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(new GetFolderSummaryQuery(), cancellationToken));

    [HttpGet("~/api/folder/{folderId:int}/detail")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(GetFolderDetailQueryResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderDetailQueryResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderDetailQueryResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderDetailQueryResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderDetailQueryResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Detail(int folderId, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(new GetFolderDetailQuery { FolderId = folderId }, cancellationToken));

    [HttpPost("~/api/folder")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(CreateFolderCommandResponse), StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFolderCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFolderCommandResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFolderCommandResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFolderCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Create([FromBody] CreateFolderCommand command, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(command, cancellationToken));

    [HttpPut("~/api/folder")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(UpdateFolderCommandResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UpdateFolderCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UpdateFolderCommandResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UpdateFolderCommandResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UpdateFolderCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Update([FromBody] UpdateFolderCommand command, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(command, cancellationToken));
}
