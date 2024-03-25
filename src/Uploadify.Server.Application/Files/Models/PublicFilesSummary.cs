using Uploadify.Server.Domain.Infrastructure.Pagination.Models;
using Uploadify.Server.Domain.Infrastructure.Pagination.Models.Files;

namespace Uploadify.Server.Application.Files.Models;

public class PublicFilesSummary : PaginationMetadata
{
    public List<PublicFileOverview>? Files { get; set; }

    public PublicFilesSummary(List<PublicFileOverview> files, int totalItems, int pageNumber, int pageSize) : base(totalItems, pageNumber, pageSize)
    {
        Files = files;
    }
}
