using Uploadify.Server.Domain.Infrastructure.Pagination.Models;

namespace Uploadify.Server.Application.Application.Models;

public class RolesSummary : PaginationMetadata
{
    public RolesSummary(IEnumerable<RoleOverview> roles, int totalItems, int pageNumber, int pageSize) : base(totalItems, pageNumber, pageSize)
    {
        Roles = [..roles];
    }

    public List<RoleOverview> Roles { get; private set; }
}
