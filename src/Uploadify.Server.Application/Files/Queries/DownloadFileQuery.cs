using System.Text.Json.Serialization;
using Mapster;
using MediatR;
using Uploadify.Server.Application.Files.Models;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Core.Files.Queries;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;

namespace Uploadify.Server.Application.Files.Queries;

public class DownloadFileQuery : IBaseRequest<DownloadFileQueryResponse>, IQuery<DownloadFileQueryResponse>//, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public int FileId { get; set; }
}

public class DownloadFileQueryHandler : IQueryHandler<DownloadFileQuery, DownloadFileQueryResponse>
{
    private readonly ISender _sender;

    public DownloadFileQueryHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task<DownloadFileQueryResponse> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
    {
        var userResponse = await _sender.Send(new GetUserQuery(request.UserName), cancellationToken);
        // if (userResponse is not { Status: Ok, User: not null })
        // {
        //     return new(userResponse);
        // }

        var fileResponse = await _sender.Send(new GetFileQuery { FileId = request.FileId }, cancellationToken);
        // if (fileResponse is not {  Status: Ok, File: not null })
        // {
        //     return new(fileResponse);
        // }

        var folderResponse = await _sender.Send(new GetFolderQuery(fileResponse.File.FolderId), cancellationToken);
        // if (folderResponse is not { Status: Ok, Folder: not null })
        // {
        //     return new(folderResponse);
        // }

        // if (folderResponse.Folder.UserId != userResponse.User.Id)
        // {
        //     return new(Unauthorized, new()
        //     {
        //         UserFriendlyMessage = Translations.RequestStatuses.Unauthorized,
        //         Exception = new UnauthorizedException(request.UserName ?? string.Empty, request.FileId.ToString(), nameof(File))
        //     });
        // }

        return new (fileResponse.File.Adapt<FileMetadata>());
    }
}

public class DownloadFileQueryResponse : BaseResponse
{
    public DownloadFileQueryResponse()
    {
    }

    public DownloadFileQueryResponse(BaseResponse? response) : base(response)
    {
    }

    public DownloadFileQueryResponse(Status status, RequestFailure? failure = null) : base(status, failure)
    {
    }

    public DownloadFileQueryResponse(FileMetadata metadata)
    {
        Status = Ok;
        Metadata = metadata;
    }

    public FileMetadata? Metadata { get; set; }
}
