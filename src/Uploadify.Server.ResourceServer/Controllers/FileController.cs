using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Uploadify.Authorization.Attributes;
using Uploadify.Server.Application.Files.Commands;
using Uploadify.Server.Application.Files.Queries;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Pagination.Models.Files;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.ResourceServer.Infrastructure.Controllers.Models;

namespace Uploadify.Server.ResourceServer.Controllers;

public class FileController : BaseApiController<FileController>
{
    public FileController(IMediator mediator, ILogger<FileController> logger) : base(mediator, logger)
    {
    }

    [HttpPost("~/api/file")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(CreateFileCommandResponse), StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFileCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFileCommandResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFileCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Upload([FromForm] CreateFileCommand command, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(command, cancellationToken));


    [HttpGet("~/api/file/{fileId:int}")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(PhysicalFileResult), StatusCodes.Status200OK, MediaTypeNames.Application.Octet)]
    [ProducesResponseType(typeof(CreateFileCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFileCommandResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFileCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Download([FromRoute] int fileId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await Mediator.Send(new DownloadFileQuery { FileId = fileId }, cancellationToken);
            if (response is not { Status: Status.Ok, Metadata: not null })
            {
                return ConvertToActionResult(response);
            }

            return PhysicalFile(
                physicalPath: response.Metadata.Location,
                contentType: response.Metadata.MimeType,
                fileDownloadName: response.Metadata.UnsafeName);
        }
        catch (Exception)
        {
            return ConvertToActionResult(new BaseResponse(Status.InternalServerError, new ()
            {
                UserFriendlyMessage = Translations.RequestStatuses.InternalServerError
            }));
        }
    }

    [HttpPut("~/api/file/rename")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(RenameFileCommandResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RenameFileCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RenameFileCommandResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RenameFileCommandResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RenameFileCommandResponse), StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RenameFileCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Rename([FromBody] RenameFileCommand command, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(command, cancellationToken));

    [HttpPut("~/api/file/move")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(MoveFileCommandResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(MoveFileCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(MoveFileCommandResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(MoveFileCommandResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(MoveFileCommandResponse), StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(MoveFileCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Move([FromBody] MoveFileCommand command, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(command, cancellationToken));

    [HttpDelete("~/api/file/{fileId:int}")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(DeleteFileCommandResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DeleteFileCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DeleteFileCommandResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DeleteFileCommandResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DeleteFileCommandResponse), StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(DeleteFileCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Delete([FromRoute] int fileId, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(new DeleteFileCommand { FileId = fileId }, cancellationToken));

    [HttpGet("~/api/files/public")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(GetAllPublicFilesQueryResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetAllPublicFilesQueryResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(GetAllPublicFilesQueryResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetAllPublic([FromQuery] FileQueryString query, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(new GetAllPublicFilesQuery(query), cancellationToken));

    [HttpPut("~/api/file/change-visibility")]
    [Permission(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(ChangeFileVisibilityCommandResponse), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ChangeFileVisibilityCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ChangeFileVisibilityCommandResponse), StatusCodes.Status401Unauthorized, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ChangeFileVisibilityCommandResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ChangeFileVisibilityCommandResponse), StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ChangeFileVisibilityCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ChangeVisibility([FromBody] ChangeFileVisibilityCommand command, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(command, cancellationToken));

}
