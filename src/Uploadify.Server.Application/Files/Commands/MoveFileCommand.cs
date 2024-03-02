using System.Text.Json.Serialization;
using Mapster;
using MediatR;
using Uploadify.Server.Application.Files.Models;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Core.Files.Queries;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Files.Models;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static System.String;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;
using File = Uploadify.Server.Domain.Files.Models.File;

namespace Uploadify.Server.Application.Files.Commands;

public class MoveFileCommand : IBaseRequest<MoveFileCommandResponse>, ICommand<MoveFileCommandResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public int? FileId { get; set; }
    public int? DestinationFolderId { get; set; }
}

public class MoveFileCommandHandler : ICommandHandler<MoveFileCommand, MoveFileCommandResponse>
{
    private readonly DataContext _context;
    private readonly ISender _sender;

    public MoveFileCommandHandler(DataContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<MoveFileCommandResponse> Handle(MoveFileCommand request, CancellationToken cancellationToken)
    {
        var userResponse = await _sender.Send(new GetUserQuery(request.UserName), cancellationToken: default);
        if (userResponse is not { Status: Ok, User: not null })
        {
            return new(userResponse);
        }

        var fileResponse = await _sender.Send(new GetFileQuery { FileId = request.FileId }, cancellationToken: default);
        if (fileResponse is not { Status: Ok, File: not null })
        {
            return new(fileResponse);
        }

        if (fileResponse.File.Folder?.UserId != userResponse.User.Id)
        {
            return new(Unauthorized, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.Unauthorized,
                Exception = new UnauthorizedException(request.UserName ?? Empty, request.FileId?.ToString() ?? Empty, nameof(File))
            });
        }

        var folderResponse = await _sender.Send(new GetFolderQuery(request.DestinationFolderId), cancellationToken: default);
        if (folderResponse is not { Status: Ok, Folder: not null })
        {
            return new(folderResponse);
        }

        if (folderResponse.Folder.UserId != userResponse.User.Id)
        {
            return new(Forbidden, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.Forbidden,
                Exception = new UnauthorizedException(request.UserName ?? Empty, request.DestinationFolderId?.ToString() ?? Empty, nameof(Folder))
            });
        }

        fileResponse.File.FolderId = folderResponse.Folder.Id;

        await _context.UpdateEntity(fileResponse.File, cancellationToken: default);
        await _context.SaveChangesAsync(cancellationToken: default);

        var overview = fileResponse.File.Adapt<FileOverview>();

        overview.CreatedBy = fileResponse.File.UserCreatedBy?.FullName;
        overview.UpdatedBy = fileResponse.File.UserUpdatedBy?.FullName;

        return new(overview);
    }
}

public class MoveFileCommandResponse : BaseResponse
{
    public MoveFileCommandResponse()
    {
    }

    public MoveFileCommandResponse(BaseResponse? response) : base(response)
    {
    }

    public MoveFileCommandResponse(Status status, RequestFailure? failure) : base(status, failure)
    {
    }

    public MoveFileCommandResponse(FileOverview file) : base(Ok) => File = file;

    public FileOverview? File { get; set; }
}
