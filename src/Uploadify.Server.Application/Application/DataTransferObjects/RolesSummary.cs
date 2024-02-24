using Uploadify.Server.Domain.Pagination.Models;

namespace Uploadify.Server.Application.Application.DataTransferObjects;

public class RolesSummary : PaginationMetadata
{
    public RolesSummary(IEnumerable<RoleOverview> roles, int totalItems, int pageNumber, int pageSize) : base(totalItems, pageNumber, pageSize)
    {
        Roles = new(roles);
    }

    public List<RoleOverview> Roles { get; private set; }
}
