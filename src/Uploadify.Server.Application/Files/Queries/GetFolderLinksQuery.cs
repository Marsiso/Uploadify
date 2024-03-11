using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Application.Files.Models;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;

namespace Uploadify.Server.Application.Files.Queries;

public class GetFolderLinksQuery : IBaseRequest<GetFolderLinksQueryResponse>, IQuery<GetFolderLinksQueryResponse>
{
    public GetFolderLinksQuery(int folderId)
    {
        FolderId = folderId;
    }

    public int FolderId { get; set; }
}

public class GetFolderLinksQueryHandler : IQueryHandler<GetFolderLinksQuery, GetFolderLinksQueryResponse>
{
    private static readonly Func<DataContext, int, Task<FolderLink>> Query = EF.CompileAsyncQuery((DataContext context, int folderId) =>
        context.Folders.Where(folder => folder.Id == folderId).Select(folder => new FolderLink
        {
            FolderId = folder.Id,
            ParentId = folder.ParentId,
            Name = folder.Name
        }).Single());

    private readonly DataContext _context;

    public GetFolderLinksQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<GetFolderLinksQueryResponse> Handle(GetFolderLinksQuery request, CancellationToken cancellationToken)
    {
        var links = new List<FolderLink>();

        do links.Add(await Query(_context, links.LastOrDefault()?.ParentId ?? request.FolderId));
        while (links.Last().ParentId.HasValue);

        links.Reverse();

        return new(links);
    }
}

public class GetFolderLinksQueryResponse : BaseResponse
{
    public GetFolderLinksQueryResponse()
    {
    }

    public GetFolderLinksQueryResponse(List<FolderLink> links) : base(Ok)
    {
        Links = links;
    }

    public List<FolderLink>? Links { get; set; }
}
