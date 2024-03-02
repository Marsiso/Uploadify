using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;
using Mapster;
using MediatR;
using Uploadify.Server.Application.Files.Models;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Core.Files.Queries;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;

namespace Uploadify.Server.Application.Files.Commands;

public class RenameFolderCommand : IBaseRequest<RenameFolderCommandResponse>, ICommand<RenameFolderCommandResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public string? Name { get; set; }
    public int? FolderId { get; set; }
}

public class RenameFolderCommandHandler : ICommandHandler<RenameFolderCommand, RenameFolderCommandResponse>
{
    private readonly DataContext _context;
    private readonly ISender _sender;

    public RenameFolderCommandHandler(DataContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<RenameFolderCommandResponse> Handle(RenameFolderCommand request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request.FolderId);
        Guard.IsNotNullOrWhiteSpace(request.Name);

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
                UserFriendlyMessage = Translations.RequestStatuses.Forbidden
            });
        }

        folderResponse.Folder.Name = request.Name;

        await _context.UpdateEntity(folderResponse.Folder, cancellationToken: default);
        await _context.SaveChangesAsync(cancellationToken: default);

        var overview = folderResponse.Folder.Adapt<FolderOverview>();

        overview.CreatedBy = folderResponse.Folder.UserCreatedBy?.FullName;
        overview.UpdatedBy = folderResponse.Folder.UserUpdatedBy?.FullName;

        return new(overview);
    }
}

public class RenameFolderCommandResponse : BaseResponse
{
    public RenameFolderCommandResponse()
    {
    }

    public RenameFolderCommandResponse(BaseResponse? response) : base(response)
    {
    }

    public RenameFolderCommandResponse(Status status, RequestFailure? failure) : base (status, failure)
    {
    }

    public RenameFolderCommandResponse(FolderOverview? folder)
    {
        Status = Ok;
        Folder = folder;
    }

    public FolderOverview? Folder { get; set; }
}
