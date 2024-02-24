using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Uploadify.Authorization.Attributes;
using Uploadify.Server.Application.FileSystem.Commands;
using Uploadify.Server.ResourceServer.Infrastructure.Controllers.Models;

namespace Uploadify.Server.ResourceServer.Controllers;

public class FileController : BaseApiController<FileController>
{
    public FileController(IMediator mediator, ILogger<FileController> logger) : base(mediator, logger)
    {
    }

    [HttpPost("~/api/file")]
    [Permission]
    [ProducesResponseType(typeof(CreateFileCommandResponse), StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFileCommandResponse), StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFileCommandResponse), StatusCodes.Status403Forbidden, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateFileCommandResponse), StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> CreateFile([FromBody] CreateFileCommand command, CancellationToken cancellationToken) => ConvertToActionResult(await Mediator.Send(command, cancellationToken));
}
