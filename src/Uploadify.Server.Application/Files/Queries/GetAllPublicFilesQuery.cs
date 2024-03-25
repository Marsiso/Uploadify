using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Application.Files.Models;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Infrastructure.Pagination.Models.Files;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static System.String;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;
using File = Uploadify.Server.Domain.Files.Models.File;

namespace Uploadify.Server.Application.Files.Queries;

public class GetAllPublicFilesQuery : IBaseRequest<GetAllPublicFilesQueryResponse>, IQuery<GetAllPublicFilesQueryResponse>
{
    public GetAllPublicFilesQuery(FileQueryString queryString)
    {
        QueryString = queryString;
    }

    public FileQueryString QueryString { get; set; }
}

public class GetAllPublicFilesQueryHandler : IQueryHandler<GetAllPublicFilesQuery, GetAllPublicFilesQueryResponse>
{
    public static readonly Func<DataContext, int, int, string, IAsyncEnumerable<PublicFileOverview>> Query = EF.CompileAsyncQuery((DataContext context, int skip, int take, string searchTerm) =>
        context.Files
            .Where(file => file.IsPublic)
            .Where(file => IsNullOrWhiteSpace(searchTerm) || file.SearchVector.Matches(EF.Functions.PlainToTsQuery("english",searchTerm)))
            .OrderBy(file => file.SearchVector.Rank(EF.Functions.PlainToTsQuery("english", searchTerm)))
            .Skip(skip)
            .Take(take)
            .Select(file => new PublicFileOverview
            {
                FileId = file.Id,
                Name = file.UnsafeName,
                OwnerUserName = file.Folder.User.UserName,
                DateCreated = file.DateCreated,
                Extension = file.Extension,
                MimeType = file.MimeType,
                Size = file.Size
            }));

    public static readonly Func<DataContext, string, Task<int>> CountQuery = EF.CompileAsyncQuery((DataContext context, string searchTerm) =>
        context.Files
            .Where(file => file.IsPublic)
            .Count(file => IsNullOrWhiteSpace(searchTerm) || file.SearchVector.Matches(EF.Functions.PlainToTsQuery("english",searchTerm))));

    private readonly DataContext _context;

    public GetAllPublicFilesQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<GetAllPublicFilesQueryResponse> Handle(GetAllPublicFilesQuery request, CancellationToken cancellationToken)
    {
        request.QueryString.SearchTerm ??= Empty;
        request.QueryString.SearchTerm = request.QueryString.SearchTerm.Trim().Normalize();

        var files = new List<PublicFileOverview>();
        await foreach (var file in Query(_context, (request.QueryString.PageNumber - 1) * request.QueryString.PageSize, request.QueryString.PageSize, request.QueryString.SearchTerm))
        {
            files.Add(file);
        }

        return new(new(
            files,
            await CountQuery(_context, request.QueryString.SearchTerm),
            request.QueryString.PageNumber,
            request.QueryString.PageSize));
    }
}

public class GetAllPublicFilesQueryResponse : BaseResponse
{
    public GetAllPublicFilesQueryResponse() : base(InternalServerError)
    {
    }

    public GetAllPublicFilesQueryResponse(PublicFilesSummary summary) : base(Ok)
    {
        Summary = summary;
    }

    public PublicFilesSummary? Summary { get; set; }
}
