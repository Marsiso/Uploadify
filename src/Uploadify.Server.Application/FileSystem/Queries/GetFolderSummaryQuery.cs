using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Application.FileSystem.DataTransferObjects;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.FileSystem.Models;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static Uploadify.Server.Domain.Requests.Models.Status;

namespace Uploadify.Server.Application.FileSystem.Queries;

public class GetFolderSummaryQuery : IQuery<GetFolderSummaryQueryResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public int? FolderId { get; set; }
}

public class GetFolderSummaryQueryHandler : IQueryHandler<GetFolderSummaryQuery, GetFolderSummaryQueryResponse>
{
    private readonly DataContext _context;
    private readonly ISender _sender;

    public GetFolderSummaryQueryHandler(DataContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<GetFolderSummaryQueryResponse> Handle(GetFolderSummaryQuery request, CancellationToken cancellationToken)
    {
        var userResponse = await _sender.Send(new GetUserQuery(request.UserName), cancellationToken);
        if (userResponse is not { Status: Ok, User: not null })
        {
            return new(userResponse);
        }

        var folder = await _context.Folders
            .Where(folder => folder.UserId == userResponse.User.Id && (request.FolderId.HasValue ? folder.Id == request.FolderId : folder.ParentId == null))
            .Include(folder => folder.Files)
            !.ThenInclude(file => file.UserCreatedBy)
            .Include(folder => folder.Files)
            !.ThenInclude(file => file.UserUpdatedBy)
            .Include(folder => folder.Children)
            !.ThenInclude(folder => folder.UserCreatedBy)
            .Include(folder => folder.Children)
            !.ThenInclude(folder => folder.UserUpdatedBy)
            .Select(folder => new FolderSummary
            {
                FolderId = folder.Id,
                Name = folder.Name,
                TotalCount = folder.TotalCount,
                TotalSize = folder.TotalSize,
                DateCreated = folder.DateCreated,
                DateUpdated = folder.DateUpdated,
                Files = folder.Files == null ? null : folder.Files.Select(file => new FileOverview
                {
                    Id = file.Id,
                    Name = file.UnsafeName,
                    MimeType = file.MimeType,
                    Extension = file.Extension,
                    Size = file.Size,
                    DateCreated = file.DateCreated,
                    DateUpdated = file.DateUpdated,
                }),
                Folders = folder.Children == null ? null : folder.Children.Select(subfolder => new FolderOverview
                {
                    FolderId = subfolder.Id,
                    Name = subfolder.Name,
                    TotalCount = subfolder.TotalCount,
                    TotalSize = subfolder.TotalSize,
                    DateCreated = subfolder.DateCreated,
                    DateUpdated = subfolder.DateUpdated
                })
            }).SingleOrDefaultAsync(cancellationToken: cancellationToken);

        if (folder == null)
        {
            return new(NotFound, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.NotFound,
                Exception = new EntityNotFoundException(request.FolderId?.ToString() ?? string.Empty, nameof(Folder))
            });
        }

        return new(folder);
    }
}

public class GetFolderSummaryQueryResponse : BaseResponse
{
    public GetFolderSummaryQueryResponse()
    {
    }

    public GetFolderSummaryQueryResponse(BaseResponse? response) : base(response)
    {
    }

    public GetFolderSummaryQueryResponse(Status status, RequestFailure? failure = null) : base(status, failure)
    {
    }

    public GetFolderSummaryQueryResponse(FolderSummary? summary)
    {
        Status = Ok;
        Summary = summary;
    }

    public FolderSummary? Summary { get; set; }
}
