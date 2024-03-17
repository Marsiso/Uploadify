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
    private readonly DataContext _context;

    public GetFolderLinksQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<GetFolderLinksQueryResponse> Handle(GetFolderLinksQuery request, CancellationToken cancellationToken)
    {
        var links = new List<FolderLink>();

        do
        {
            var folderId = links.LastOrDefault()?.ParentId ?? request.FolderId;
            links.Add(await _context.Folders.Select(folder => new FolderLink
            {
                FolderId = folder.Id,
                ParentId = folder.ParentId,
                Name = folder.Name
            }).SingleAsync(folder => folder.FolderId == folderId, cancellationToken));
        } while (links.Last().ParentId.HasValue);

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
