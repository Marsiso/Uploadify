using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;
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
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;

namespace Uploadify.Server.Application.Files.Commands;

public class CreateFolderCommand : IBaseRequest<CreateFolderCommandResponse>, ICommand<CreateFolderCommandResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public string? Name { get; set; }
    public int? CategoryId { get; set; }
    public int? ParentId { get; set; }
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
        Guard.IsNotNull(request.ParentId);
        Guard.IsNotNullOrWhiteSpace(request.Name);

        var userResponse = await _sender.Send(new GetUserQuery(request.UserName), cancellationToken: default);
        if (userResponse is not { Status: Ok, User: not null })
        {
            return new(userResponse);
        }

        var folderResponse = await _sender.Send(new GetFolderQuery(request.ParentId), cancellationToken: default);
        if (folderResponse is not { Status: Ok, Folder: not null })
        {
            return new(folderResponse);
        }

        if (folderResponse.Folder.UserId != userResponse.User.Id)
        {
            return new(Unauthorized, new()
            {
                Exception = new UnauthorizedException(request.UserName, request.ParentId.ToString(), nameof(Folder)),
                UserFriendlyMessage = Translations.RequestStatuses.Unauthorized
            });
        }

        var folder = request.Adapt<Folder>();

        folder.UserId = userResponse.User.Id;

        await _context.AddAsync(folder, cancellationToken: default);
        await _context.SaveChangesAsync(cancellationToken: default);

        var overview = folder.Adapt<FolderOverview>();

        overview.CreatedBy = folder.UserCreatedBy?.FullName;
        overview.UpdatedBy = folder.UserUpdatedBy?.FullName;

        return new(overview);
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

    public CreateFolderCommandResponse(FolderOverview folder) : base(Created)
    {
        Folder = folder;
    }

    public FolderOverview? Folder { get; set; }
}
