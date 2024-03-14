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

public class DeleteFolderCommand : IBaseRequest<DeleteFolderCommandResponse>, ICommand<DeleteFolderCommandResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public int? FolderId { get; set; }
}

public class DeleteFolderCommandHandler : ICommandHandler<DeleteFolderCommand, DeleteFolderCommandResponse>
{
    private readonly DataContext _context;
    private readonly ISender _sender;

    public DeleteFolderCommandHandler(DataContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<DeleteFolderCommandResponse> Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
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
                Exception = new UnauthorizedException(request.UserName ?? Empty, request.FolderId?.ToString() ?? Empty, nameof(Folder))
            });
        }

        foreach (var folder in folderResponse.Folder.Children ?? Enumerable.Empty<Folder>())
        {
            var deleteFolderResponse = await _sender.Send(new DeleteFolderCommand { FolderId = folder.Id }, cancellationToken: default);
            if (deleteFolderResponse is not { Status: Ok })
            {
                return new(deleteFolderResponse);
            }
        }

        foreach (var file in folderResponse.Folder.Files ?? Enumerable.Empty<File>())
        {
            await _context.DeleteEntity(file, cancellationToken: default);
        }

        await _context.DeleteEntity(folderResponse.Folder, cancellationToken: default);
        await _context.SaveChangesAsync(cancellationToken: default);

        var overview = folderResponse.Folder.Adapt<FolderOverview>();

        overview.CreatedBy = folderResponse.Folder.UserCreatedBy?.FullName;
        overview.UpdatedBy = folderResponse.Folder.UserUpdatedBy?.FullName;

        return new(overview);
    }
}

public class DeleteFolderCommandResponse : BaseResponse
{
    public DeleteFolderCommandResponse()
    {
    }

    public DeleteFolderCommandResponse(BaseResponse? response) : base(response)
    {
    }

    public DeleteFolderCommandResponse(Status status, RequestFailure? failure) : base(status, failure)
    {
    }

    public DeleteFolderCommandResponse(FolderOverview? folder) : base(Ok) => Folder = folder;

    public FolderOverview? Folder { get; set; }
}
