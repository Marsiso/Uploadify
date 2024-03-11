using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Uploadify.Authorization.Attributes;
using Uploadify.Server.Application.Files.Commands;
using Uploadify.Server.Application.Files.Queries;
using Uploadify.Server.ResourceServer.Infrastructure.Controllers.Models;

namespace Uploadify.Server.ResourceServer.Controllers;

public class FolderController : BaseApiController<FolderController>
{
    public FolderController(IMediator mediator, ILogger<FolderController> logger) : base(mediator, logger)
    {
    }

    [HttpGet("~/api/folder/summary")]
    [HttpGet("~/api/folder/{folderId:int}/summary")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(GetFolderSummaryQueryResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderSummaryQueryResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderSummaryQueryResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderSummaryQueryResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderSummaryQueryResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Summary([FromRoute] int? folderId, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(new GetFolderSummaryQuery { FolderId = folderId }, cancellationToken));

    [HttpGet("~/api/folder/{folderId:int}/detail")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(GetFolderDetailQueryResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderDetailQueryResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderDetailQueryResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderDetailQueryResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetFolderDetailQueryResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Detail([FromRoute] int folderId, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(new GetFolderDetailQuery { FolderId = folderId }, cancellationToken));

    [HttpPost("~/api/folder")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(CreateFolderCommandResponse), StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFolderCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFolderCommandResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFolderCommandResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFolderCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Create([FromBody] CreateFolderCommand command, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(command, cancellationToken));

    [HttpPut("~/api/folder/rename")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(RenameFolderCommandResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RenameFolderCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RenameFolderCommandResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RenameFolderCommandResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RenameFolderCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Rename([FromBody] RenameFolderCommand command, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(command, cancellationToken));

    [HttpPut("~/api/folder/move")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(MoveFolderCommandResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(MoveFolderCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(MoveFolderCommandResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(MoveFolderCommandResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(MoveFolderCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Move([FromBody] MoveFolderCommand command, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(command, cancellationToken));

    [HttpDelete("~/api/folder/{folderId:int}")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(DeleteFolderCommandResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DeleteFolderCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DeleteFolderCommandResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DeleteFolderCommandResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DeleteFolderCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Delete([FromRoute] int folderId, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(new DeleteFolderCommand { FolderId = folderId }, cancellationToken));

}
