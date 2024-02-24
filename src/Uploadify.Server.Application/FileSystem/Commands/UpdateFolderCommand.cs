using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;
using MediatR;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Core.FileSystem.Queries;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.FileSystem.Models;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static Uploadify.Server.Domain.Requests.Models.Status;

namespace Uploadify.Server.Application.FileSystem.Commands;

public class UpdateFolderCommand : ICommand<UpdateFolderCommandResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public string? Name { get; set; }
    public int? FolderId { get; set; }
}

public class UpdateFolderCommandHandler : ICommandHandler<UpdateFolderCommand, UpdateFolderCommandResponse>
{
    private readonly DataContext _context;
    private readonly ISender _sender;

    public UpdateFolderCommandHandler(DataContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<UpdateFolderCommandResponse> Handle(UpdateFolderCommand request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request.FolderId);
        Guard.IsNotNullOrWhiteSpace(request.Name);

        var userResponse = await _sender.Send(new GetUserQuery(request.UserName), cancellationToken);
        if (userResponse is not { Status: Ok, User: not null })
        {
            return new(userResponse);
        }

        var folderResponse = await _sender.Send(new GetFolderQuery(request.FolderId), cancellationToken);
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

        _context.Update(folderResponse.Folder);
        await _context.SaveChangesAsync(cancellationToken);

        return new(folderResponse.Folder);
    }
}

public class UpdateFolderCommandResponse : BaseResponse
{
    public UpdateFolderCommandResponse()
    {
    }

    public UpdateFolderCommandResponse(BaseResponse? response) : base(response)
    {
    }

    public UpdateFolderCommandResponse(Status status, RequestFailure? failure) : base (status, failure)
    {
    }

    public UpdateFolderCommandResponse(Folder? folder)
    {
        Status = Ok;
        Folder = folder;
    }

    public Folder? Folder { get; set; }
}
