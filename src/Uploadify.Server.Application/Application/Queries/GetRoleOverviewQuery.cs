using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Application.Application.DataTransferObjects;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static System.String;
using static Uploadify.Server.Domain.Requests.Models.Status;

namespace Uploadify.Server.Application.Application.Queries;

public class GetRoleOverviewQuery : BaseRequest<GetRoleOverviewQueryResponse>, IQuery<GetRoleOverviewQueryResponse>
{
    public GetRoleOverviewQuery(string roleID)
    {
        RoleID = roleID;
    }

    public string? RoleID { get; set; }
}

public class GetRoleOverviewQueryHandler : IQueryHandler<GetRoleOverviewQuery, GetRoleOverviewQueryResponse>
{
    public static readonly Func<DataContext, string, Task<RoleOverview?>> Query = EF.CompileAsyncQuery((DataContext context, string id) =>
        context.Roles.Where(role => role.Id == id)
            .Include(role => role.UserCreatedBy)
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
            })
            .SingleOrDefault());

    private readonly DataContext _context;

    public GetRoleOverviewQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<GetRoleOverviewQueryResponse> Handle(GetRoleOverviewQuery request, CancellationToken cancellationToken)
    {
        if (IsNullOrWhiteSpace(request.RoleID))
        {
            return new(BadRequest, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                Exception = new BadRequestException(nameof(request.RoleID))
            });
        }

        var overview = await Query(_context, request.RoleID);
        if (overview == null)
        {
            return new(NotFound, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.NotFound,
                Exception = new EntityNotFoundException(request.RoleID, nameof(Role))
            });
        }

        return new(overview);
    }
}

public class GetRoleOverviewQueryResponse : BaseResponse
{
    public GetRoleOverviewQueryResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public GetRoleOverviewQueryResponse(RoleOverview? overview) : base(Ok)
    {
        Status = Ok;
        Overview = overview;
    }

    public RoleOverview? Overview { get; set; }
}
