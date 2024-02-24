using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.FileSystem.Models;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static Uploadify.Server.Domain.Requests.Models.Status;

namespace Uploadify.Server.Core.FileSystem.Queries;

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
    private static readonly Func<DataContext, int, Task<Folder?>> Query = EF.CompileAsyncQuery((DataContext context, int id) =>
        context.Folders.Include(folder => folder.UserCreatedBy)
            .Include(folder => folder.UserUpdatedBy)
            .SingleOrDefault(folder => folder.Id == id));

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

        var folder = await Query(_context, request.FolderId.Value);
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
    public GetFolderQueryResponse(Status status, RequestFailure? failure) : base(status, failure)
    {
    }

    public GetFolderQueryResponse(Folder? folder)
    {
        Status = Ok;
        Folder = folder;
    }

    public Folder? Folder { get; set; }
}
