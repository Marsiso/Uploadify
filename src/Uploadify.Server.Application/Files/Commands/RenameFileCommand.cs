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
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static System.String;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;
using File = Uploadify.Server.Domain.Files.Models.File;

namespace Uploadify.Server.Application.Files.Commands;

public class RenameFileCommand : IBaseRequest<RenameFileCommandResponse>, ICommand<RenameFileCommandResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public int? FileId { get; set; }
    public string? Name { get; set; }
}

public class RenameFileCommandHandler : ICommandHandler<RenameFileCommand, RenameFileCommandResponse>
{
    private readonly DataContext _context;
    private readonly ISender _sender;

    public RenameFileCommandHandler(DataContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<RenameFileCommandResponse> Handle(RenameFileCommand request, CancellationToken cancellationToken)
    {
        Guard.IsNotNullOrWhiteSpace(request.Name);

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

        fileResponse.File.UnsafeName = request.Name;

        await _context.UpdateEntity(fileResponse.File, cancellationToken: default);
        await _context.SaveChangesAsync(cancellationToken: default);

        var overview = fileResponse.File.Adapt<FileOverview>();

        overview.CreatedBy = fileResponse.File.UserCreatedBy?.FullName;
        overview.UpdatedBy = fileResponse.File.UserUpdatedBy?.FullName;

        return new(overview);
    }
}

public class RenameFileCommandResponse : BaseResponse
{
    public RenameFileCommandResponse()
    {
    }

    public RenameFileCommandResponse(BaseResponse? response) : base(response)
    {
    }

    public RenameFileCommandResponse(Status status, RequestFailure? failure) : base(status, failure)
    {
    }

    public RenameFileCommandResponse(FileOverview file) : base(Ok) => File = file;

    public FileOverview? File { get; set; }
}
