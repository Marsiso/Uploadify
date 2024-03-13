using System.Text.Json.Serialization;
using MediatR;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Core.Files.Queries;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static System.String;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;

namespace Uploadify.Server.Application.Files.Commands;

public class ChangeFileVisibilityCommand : IBaseRequest<ChangeFileVisibilityCommandResponse>, ICommand<ChangeFileVisibilityCommandResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public int? FileId { get; set; }
    public bool Visibility { get; set; }
}

public class ChangeFileVisibilityCommandHandler : ICommandHandler<ChangeFileVisibilityCommand, ChangeFileVisibilityCommandResponse>
{
    private readonly DataContext _context;
    private readonly ISender _sender;

    public ChangeFileVisibilityCommandHandler(DataContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<ChangeFileVisibilityCommandResponse> Handle(ChangeFileVisibilityCommand request, CancellationToken cancellationToken)
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

        fileResponse.File.IsPublic = request.Visibility;

        await _context.UpdateEntity(fileResponse.File, cancellationToken: default);
        await _context.SaveChangesAsync(cancellationToken: default);

        return new();
    }
}


public class ChangeFileVisibilityCommandResponse : BaseResponse
{
    public ChangeFileVisibilityCommandResponse() : base(Ok)
    {
    }

    public ChangeFileVisibilityCommandResponse(BaseResponse response) : base(response)
    {
    }

    public ChangeFileVisibilityCommandResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }
}
