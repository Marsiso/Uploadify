using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Application.Application.Models;
using Uploadify.Server.Application.Files.Models;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Files.Models;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;

namespace Uploadify.Server.Application.Files.Queries;

public class GetFolderDetailQuery : IBaseRequest<GetFolderDetailQueryResponse>, IQuery<GetFolderDetailQueryResponse>, IRequestWithUserName
{
    [JsonIgnore]
    public string? UserName { get; set; }

    public int FolderId { get; set; }
}

public class GetFolderDetailQueryHandler : IQueryHandler<GetFolderDetailQuery, GetFolderDetailQueryResponse>
{
    private readonly Func<DataContext, int, Task<FolderDetail?>> Query = EF.CompileAsyncQuery((DataContext context, int folderId) =>
        context.Folders
            .Where(folder => folder.Id == folderId)
            .Include(folder => folder.UserCreatedBy)
            .Include(folder => folder.UserUpdatedBy)
            .Select(folder => new FolderDetail
            {
                Id = folder.Id,
                ParentId = folder.ParentId,
                UserId = folder.UserId,
                CategoryId = folder.CategoryId,
                Name = folder.Name,
                TotalCount = folder.TotalCount,
                TotalSize = folder.TotalSize,
                DateCreated = folder.DateCreated,
                DateUpdated = folder.DateUpdated,
                UserCreatedBy = folder.UserCreatedBy == null ? null : new UserOverview
                {
                    GivenName = folder.UserCreatedBy.GivenName,
                    FamilyName = folder.UserCreatedBy.FamilyName,
                    Email = folder.UserCreatedBy.Email,
                    PhoneNumber = folder.UserCreatedBy.PhoneNumber,
                    Picture = folder.UserCreatedBy.Picture
                },
                UserUpdatedBy = folder.UserUpdatedBy == null ? null : new UserOverview
                {
                    GivenName = folder.UserUpdatedBy.GivenName,
                    FamilyName = folder.UserUpdatedBy.FamilyName,
                    Email = folder.UserUpdatedBy.Email,
                    PhoneNumber = folder.UserUpdatedBy.PhoneNumber,
                    Picture = folder.UserUpdatedBy.Picture
                }
            })
            .SingleOrDefault());

    private readonly DataContext _context;
    private readonly ISender _sender;

    public GetFolderDetailQueryHandler(DataContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<GetFolderDetailQueryResponse> Handle(GetFolderDetailQuery request, CancellationToken cancellationToken)
    {
        var userResponse = await _sender.Send(new GetUserQuery(request.UserName), cancellationToken);
        if (userResponse is not { Status: Ok, User: not null })
        {
            return new(userResponse);
        }

        var folder = await Query(_context, request.FolderId);
        if (folder == null)
        {
            return new(NotFound, new() { UserFriendlyMessage = Translations.RequestStatuses.NotFound, Exception = new EntityNotFoundException(request.FolderId.ToString(), nameof(Folder))});
        }

        if (folder.UserId != userResponse.User.Id)
        {
            return new(Unauthorized, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.Unauthorized,
                Exception = new UnauthorizedException(request.UserName ?? string.Empty, request.FolderId.ToString(), nameof(Folder))
            });
        }

        return new(folder);
    }
}

public class GetFolderDetailQueryResponse : BaseResponse
{
    public GetFolderDetailQueryResponse()
    {
    }

    public GetFolderDetailQueryResponse(BaseResponse? response) : base(response)
    {
    }

    public GetFolderDetailQueryResponse(Status status, RequestFailure? failure = null) : base(status, failure)
    {
    }

    public GetFolderDetailQueryResponse(FolderDetail? folder)
    {
        Status = Ok;
        Folder = folder;
    }

    public FolderDetail? Folder { get; set; }
}
