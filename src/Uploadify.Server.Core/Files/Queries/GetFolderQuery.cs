﻿using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Files.Models;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;

namespace Uploadify.Server.Core.Files.Queries;

public class GetFolderQuery : IQuery<GetFolderQueryResponse>
{
    public GetFolderQuery(int? folderId)
    {
        FolderId = folderId;
    }

    public GetFolderQuery()
    {
    }

    public int? FolderId { get; set; }
}

public class GetFolderQueryHandler : IQueryHandler<GetFolderQuery, GetFolderQueryResponse>
{
    private readonly DataContext _context;

    public GetFolderQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<GetFolderQueryResponse> Handle(GetFolderQuery request, CancellationToken cancellationToken)
    {
        if (!request.FolderId.HasValue)
        {
            return new(BadRequest, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                Exception = new BadRequestException(nameof(GetFolderQuery))
            });
        }

        var folder = await _context.Folders
            .Include(folder => folder.Files)
            .Include(folder => folder.Children)
            .Include(folder => folder.UserCreatedBy)
            .Include(folder => folder.UserUpdatedBy)
            .SingleOrDefaultAsync(folder => folder.Id == request.FolderId.Value, cancellationToken: cancellationToken);

        if (folder == null)
        {
            return new(NotFound, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.NotFound,
                Exception = new EntityNotFoundException(request.FolderId.Value.ToString(), nameof(Folder))
            });
        }

        return new(folder);
    }
}

public class GetFolderQueryResponse : BaseResponse
{
    public GetFolderQueryResponse()
    {
    }

    public GetFolderQueryResponse(Status status, RequestFailure? failure) : base(status, failure)
    {
    }

    public GetFolderQueryResponse(Folder folder) : base(Ok)
    {
        Folder = folder;
    }

    public Folder? Folder { get; set; }
}
