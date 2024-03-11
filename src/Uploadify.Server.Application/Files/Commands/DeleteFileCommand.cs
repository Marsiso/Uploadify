using System.Text.Json.Serialization;
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
using File = Uploadify.Server.Domain.Files.Models.File;
using static System.String;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;

namespace Uploadify.Server.Application.Files.Commands;

public class DeleteFileCommand : IBaseRequest<DeleteFileCommandResponse>, ICommand<DeleteFileCommandResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public int? FileId { get; set; }
}

public class DeleteFileCommandHandler : ICommandHandler<DeleteFileCommand, DeleteFileCommandResponse>
{
    private readonly DataContext _context;
    private readonly ISender _sender;

    public DeleteFileCommandHandler(DataContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<DeleteFileCommandResponse> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
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

        await _context.DeleteEntity(fileResponse.File, cancellationToken: default);
        await _context.SaveChangesAsync(cancellationToken: default);

        var overview = fileResponse.File.Adapt<FileOverview>();

        overview.CreatedBy = fileResponse.File.UserCreatedBy?.FullName;
        overview.UpdatedBy = fileResponse.File.UserUpdatedBy?.FullName;

        return new(overview);
    }
}

public class DeleteFileCommandResponse : BaseResponse
{
    public DeleteFileCommandResponse()
    {
    }

    public DeleteFileCommandResponse(BaseResponse? response) : base(response)
    {
    }

    public DeleteFileCommandResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public DeleteFileCommandResponse(FileOverview file) : base(Ok) => File = file;

    public FileOverview? File { get; set; }
}
