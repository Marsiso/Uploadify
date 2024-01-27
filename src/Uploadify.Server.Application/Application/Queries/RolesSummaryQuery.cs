﻿using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Application.Application.DataTransferObjects;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Pagination.Models;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static Uploadify.Server.Domain.Requests.Models.Status;

namespace Uploadify.Server.Application.Application.Queries;

public class RolesSummaryQuery : BaseRequest<RolesSummaryQueryResponse>, IQuery<RolesSummaryQueryResponse>
{
    public RolesSummaryQuery(RoleQueryString queryString)
    {
        QueryString = queryString;
    }

    public RoleQueryString QueryString { get; set; }
}

public class RolesSummaryQueryHandler : IQueryHandler<RolesSummaryQuery, RolesSummaryQueryResponse>
{
    public static readonly Func<DataContext, int, int, IAsyncEnumerable<RoleOverview>> GetQuery = EF.CompileAsyncQuery((DataContext context, int skip, int take) =>
        context.Roles.Include(role => role.UserCreatedBy)
            .Include(role => role.UserUpdatedBy)
            .Select(role => new RoleOverview
            {
                Name = role.Name,
                Permission = role.Permission,
                UserCreatedBy = role.UserCreatedBy == null ? null : new UserOverview
                {
                  UserName = role.UserCreatedBy.UserName,
                  Email = role.UserCreatedBy.Email,
                  PhoneNumber = role.UserCreatedBy.PhoneNumber,
                  GivenName = role.UserCreatedBy.GivenName,
                  FamilyName = role.UserCreatedBy.FamilyName,
                  Picture = role.UserCreatedBy.Picture
                },
                DateCreated = role.DateCreated,
                UserUpdatedBy = role.UserUpdatedBy == null ? null : new UserOverview
                {
                    UserName = role.UserUpdatedBy.UserName,
                    Email = role.UserUpdatedBy.Email,
                    PhoneNumber = role.UserUpdatedBy.PhoneNumber,
                    GivenName = role.UserUpdatedBy.GivenName,
                    FamilyName = role.UserUpdatedBy.FamilyName,
                    Picture = role.UserUpdatedBy.Picture
                },
                DateUpdated = role.DateUpdated,
            }));

    public static readonly Func<DataContext, Task<int>> CountQuery = EF.CompileAsyncQuery((DataContext context) => context.Roles.Count());

    private readonly DataContext _context;

    public RolesSummaryQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<RolesSummaryQueryResponse> Handle(RolesSummaryQuery request, CancellationToken cancellationToken)
    {
        var roles = new List<RoleOverview>();
        await foreach (var overview in GetQuery(_context, request.QueryString.PageNumber - 1, request.QueryString.PageSize))
        {
            roles.Add(overview);
        }

        return new RolesSummaryQueryResponse(new RolesSummary(roles, await CountQuery(_context), request.QueryString.PageNumber, request.QueryString.PageSize));
    }
}

public class RolesSummaryQueryResponse : BaseResponse
{
    public RolesSummaryQueryResponse(RolesSummary summary)
    {
        Status = Ok;
        Summary = summary;
    }

    public RolesSummary Summary { get; set; }
}
