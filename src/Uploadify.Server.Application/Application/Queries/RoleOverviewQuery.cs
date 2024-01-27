using MediatR;
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

public class RoleOverviewQuery : BaseRequest<RoleOverviewQueryResponse>, IQuery<RoleOverviewQueryResponse>
{
    public RoleOverviewQuery(string roleID)
    {
        RoleID = roleID;
    }

    public string? RoleID { get; set; }
}

public class RoleOverviewQueryHandler : IQueryHandler<RoleOverviewQuery, RoleOverviewQueryResponse>
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

    public RoleOverviewQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<RoleOverviewQueryResponse> Handle(RoleOverviewQuery request, CancellationToken cancellationToken)
    {
        if (IsNullOrWhiteSpace(request.RoleID))
        {
            return new RoleOverviewQueryResponse(BadRequest, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                Exception = new BadRequestException(nameof(request.RoleID))
            });
        }

        var overview = await Query(_context, request.RoleID);
        if (overview == null)
        {
            return new RoleOverviewQueryResponse(NotFound, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.NotFound,
                Exception = new EntityNotFoundException(request.RoleID, nameof(Role))
            });
        }

        return new RoleOverviewQueryResponse(overview);
    }
}

public class RoleOverviewQueryResponse : BaseResponse
{
    public RoleOverviewQueryResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public RoleOverviewQueryResponse(RoleOverview? overview)
    {
        Status = Ok;
        Overview = overview;
    }

    public RoleOverview? Overview { get; set; }
}
