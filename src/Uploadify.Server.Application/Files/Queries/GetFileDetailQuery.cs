using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Application.Application.Models;
using Uploadify.Server.Application.Files.Models;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Core.Files.Queries;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;
using File = Uploadify.Server.Domain.Files.Models.File;

namespace Uploadify.Server.Application.Files.Queries;

public class GetFileDetailQuery : IBaseRequest<GetFileDetailQueryResponse>, IQuery<GetFileDetailQueryResponse>, IRequestWithUserName
{
    [JsonInclude]
    public string? UserName { get; set; }

    public int FileId { get; set; }
}

public class GetFileDetailQueryHandler : IRequestHandler<GetFileDetailQuery, GetFileDetailQueryResponse>
{
    private readonly Func<DataContext, int, Task<FileDetail?>> Query = EF.CompileAsyncQuery((DataContext context, int fileId) =>
        context.Files
            .Where(file => file.Id == fileId)
            .Include(file => file.Folder)
            .Include(file => file.UserCreatedBy)
            .Include(file => file.UserUpdatedBy)
            .Select(file => new FileDetail
            {
                Id = file.Id,
                FolderId = file.FolderId,
                CategoryId = file.CategoryId,
                Name = file.UnsafeName,
                Extension = file.Extension,
                Size = file.Size,
                DateCreated = file.DateCreated,
                DateUpdated = file.DateUpdated,
                UserCreatedBy = file.UserCreatedBy == null ? null : new UserOverview
                {
                    GivenName = file.UserCreatedBy.GivenName,
                    FamilyName = file.UserCreatedBy.FamilyName,
                    Email = file.UserCreatedBy.Email,
                    PhoneNumber = file.UserCreatedBy.PhoneNumber,
                    Picture = file.UserCreatedBy.Picture
                },
                UserUpdatedBy = file.UserUpdatedBy == null ? null : new UserOverview
                {
                    GivenName = file.UserUpdatedBy.GivenName,
                    FamilyName = file.UserUpdatedBy.FamilyName,
                    Email = file.UserUpdatedBy.Email,
                    PhoneNumber = file.UserUpdatedBy.PhoneNumber,
                    Picture = file.UserUpdatedBy.Picture
                }
            })
            .SingleOrDefault());

    private readonly DataContext _context;
    private readonly ISender _sender;

    public GetFileDetailQueryHandler(DataContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<GetFileDetailQueryResponse> Handle(GetFileDetailQuery request, CancellationToken cancellationToken)
    {
        var userResponse = await _sender.Send(new GetUserQuery(request.UserName), cancellationToken);
        if (userResponse is not { Status: Ok, User: not null })
        {
            return new(userResponse);
        }

        var file = await Query(_context, request.FileId);
        if (file == null)
        {
            return new(NotFound, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.NotFound,
                Exception = new EntityNotFoundException(request.FileId.ToString(), nameof(File))
            });
        }

        var folderResponse = await _sender.Send(new GetFolderQuery(file.FolderId), cancellationToken);
        if (folderResponse is not { Status: Ok, Folder: not null })
        {
            return new(folderResponse);
        }

        if (folderResponse.Folder.UserId != userResponse.User.Id)
        {
            return new(Unauthorized, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.Unauthorized,
                Exception = new UnauthorizedException(request.UserName ?? string.Empty, request.FileId.ToString(), nameof(File))
            });
        }

        return new(file);
    }
}

public class GetFileDetailQueryResponse : BaseResponse
{
    public GetFileDetailQueryResponse()
    {
    }

    public GetFileDetailQueryResponse(BaseResponse? response) : base(response)
    {
    }

    public GetFileDetailQueryResponse(Status status, RequestFailure? failure = null) : base(status, failure)
    {
    }

    public GetFileDetailQueryResponse(FileDetail? file)
    {
        Status = Ok;
        File = file;
    }

    public FileDetail? File { get; set; }
}
