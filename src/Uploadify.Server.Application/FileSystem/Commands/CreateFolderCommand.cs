using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;
using Mapster;
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

public class CreateFolderCommand : ICommand<CreateFolderCommandResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public string? Name { get; set; }
    public int? CategoryId { get; set; }
    public int? ParentFolderId { get; set; }
}

public class CreateFolderCommandHandler : ICommandHandler<CreateFolderCommand, CreateFolderCommandResponse>
{
    private readonly DataContext _context;
    private readonly ISender _sender;

    public CreateFolderCommandHandler(DataContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<CreateFolderCommandResponse> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request.ParentFolderId);
        Guard.IsNotNullOrWhiteSpace(request.Name);

        var userResponse = await _sender.Send(new GetUserQuery(request.UserName), cancellationToken);
        if (userResponse is not { Status: Ok, User: not null })
        {
            return new(userResponse);
        }

        var folderResponse = await _sender.Send(new GetFolderQuery(request.ParentFolderId), cancellationToken);
        if (folderResponse is not { Status: Ok, Folder: not null })
        {
            return new(folderResponse);
        }

        if (folderResponse.Folder.UserId != userResponse.User.Id)
        {
            return new(Forbidden, new() { UserFriendlyMessage = Translations.RequestStatuses.Forbidden });
        }

        var folder = request.Adapt<Folder>();

        folder.UserId = userResponse.User.Id;

        await _context.AddAsync(folder, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new(folder);
    }
}

public class CreateFolderCommandResponse : BaseResponse
{
    public CreateFolderCommandResponse()
    {
    }

    public CreateFolderCommandResponse(BaseResponse? response) : base(response)
    {
    }

    public CreateFolderCommandResponse(Status status, RequestFailure? failure) : base(status, failure)
    {
    }

    public CreateFolderCommandResponse(Folder? folder) : base(Created) => Folder = folder;

    public Folder? Folder { get; set; }
}
