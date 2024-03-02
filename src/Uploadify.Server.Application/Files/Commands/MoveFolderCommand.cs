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

namespace Uploadify.Server.Application.Files.Commands;

public class MoveFolderCommand : IBaseRequest<MoveFolderCommandResponse>, ICommand<MoveFolderCommandResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public int FolderId { get; set; }
    public int DestinationFolderId { get; set; }
}

public class MoveFolderCommandHandler : ICommandHandler<MoveFolderCommand, MoveFolderCommandResponse>
{
    private readonly DataContext _context;
    private readonly ISender _sender;

    public MoveFolderCommandHandler(DataContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<MoveFolderCommandResponse> Handle(MoveFolderCommand request, CancellationToken cancellationToken)
    {
        var userResponse = await _sender.Send(new GetUserQuery(request.UserName), cancellationToken: default);
        if (userResponse is not { Status: Ok, User: not null })
        {
            return new(userResponse);
        }

        var folderResponse = await _sender.Send(new GetFolderQuery(request.FolderId), cancellationToken: default);
        if (folderResponse is not { Status: Ok, Folder: not null })
        {
            return new(folderResponse);
        }

        if (folderResponse.Folder.UserId != userResponse.User.Id)
        {
            return new(Forbidden, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.Forbidden,
                Exception = new UnauthorizedException(request.UserName ?? Empty, request.FolderId.ToString() ?? Empty, nameof(Folder))
            });
        }

        var destinationFolderResponse = await _sender.Send(new GetFolderQuery(request.DestinationFolderId), cancellationToken: default);
        if (destinationFolderResponse is not { Status: Ok, Folder: not null })
        {
            return new(destinationFolderResponse);
        }

        if (destinationFolderResponse.Folder.UserId != userResponse.User.Id)
        {
            return new(Forbidden, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.Forbidden,
                Exception = new UnauthorizedException(request.UserName ?? Empty, request.DestinationFolderId.ToString() ?? Empty, nameof(Folder))
            });
        }

        folderResponse.Folder.ParentId = destinationFolderResponse.Folder.Id;

        await _context.UpdateEntity(folderResponse.Folder, cancellationToken: default);
        await _context.SaveChangesAsync(cancellationToken: default);

        var overview = folderResponse.Folder.Adapt<FolderOverview>();

        overview.CreatedBy = folderResponse.Folder.UserCreatedBy?.FullName;
        overview.UpdatedBy = folderResponse.Folder.UserUpdatedBy?.FullName;

        return new(overview);
    }
}

public class MoveFolderCommandResponse : BaseResponse
{
    public MoveFolderCommandResponse()
    {
    }

    public MoveFolderCommandResponse(BaseResponse? response) : base(response)
    {
    }

    public MoveFolderCommandResponse(Status status, RequestFailure? failure) : base(status, failure)
    {
    }

    public MoveFolderCommandResponse(FolderOverview folder) : base(Ok) => Folder = folder;

    public FolderOverview? Folder { get; set; }
}
