using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;
using MediatR;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Core.FileSystem.Queries;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static Uploadify.Server.Domain.Requests.Models.Status;
using File = Uploadify.Server.Domain.FileSystem.Models.File;

namespace Uploadify.Server.Application.FileSystem.Commands;

public class CreateFileCommand : ICommand<CreateFileCommandResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public int? FolderId { get; set; }
    public int? CategoryId { get; set; }
    public string? Name { get; set; }
}

public class CreateFileCommandHandler : ICommandHandler<CreateFileCommand, CreateFileCommandResponse>
{
    private readonly DataContext _context;
    private readonly ISender _sender;

    public CreateFileCommandHandler(DataContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<CreateFileCommandResponse> Handle(CreateFileCommand request, CancellationToken cancellationToken)
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
            return new(Forbidden, new() { UserFriendlyMessage = Translations.RequestStatuses.Forbidden} );
        }

        var file = new File
        {
            FolderId = request.FolderId.Value,
            UnsafeName = request.Name
        };

        await _context.AddAsync(file, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new(file);
    }
}

public class CreateFileCommandResponse : BaseResponse
{
    public CreateFileCommandResponse()
    {
    }

    public CreateFileCommandResponse(BaseResponse? response) : base(response)
    {
    }

    public CreateFileCommandResponse(Status status, RequestFailure? failure = null) : base(status, failure)
    {
    }

    public CreateFileCommandResponse(File? file) : base(Created)
    {
        File = file;
    }

    public File? File { get; set; }
}
