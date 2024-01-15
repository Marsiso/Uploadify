using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Application.Application.DataTransferObjects;
using Uploadify.Server.Core.Application.Queries;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Localization;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static System.String;
using static Uploadify.Server.Domain.Requests.Models.Status;

namespace Uploadify.Server.Application.Application.Queries;

public class GetUserDetailQuery : BaseRequest<GetUserDetailQueryResponse>, IQuery<GetUserDetailQueryResponse>
{
    public GetUserDetailQuery(string? userId)
    {
        UserID = userId;
    }

    public string? UserID { get; set; }
}

public class GetUserDetailQueryHandler : IQueryHandler<GetUserDetailQuery, GetUserDetailQueryResponse>
{
    private static readonly Func<DataContext, string, Task<UserDetail?>> Query = EF.CompileAsyncQuery((DataContext context, string id) =>
        context.Users.Where(user => user.Id == id)
            .Select(user => new UserDetail
            {
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                GivenName = user.GivenName,
                FamilyName = user.FamilyName,
                Picture = user.Picture,
                CreatedBy = user.CreatedBy,
                UpdatedBy = user.UpdatedBy
            }).SingleOrDefault());

    private readonly DataContext _context;

    public GetUserDetailQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<GetUserDetailQueryResponse> Handle(GetUserDetailQuery request, CancellationToken cancellationToken)
    {
        if (IsNullOrWhiteSpace(request.UserID))
        {
            return new GetUserDetailQueryResponse
            {
                Status = BadRequest,
                Failure = new RequestFailure
                {
                    UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                    Exception = new BadRequestException(nameof(request.UserID))
                }
            };
        }

        var userDetail = await Query(_context, request.UserID);
        if (userDetail == null)
        {
            return new GetUserDetailQueryResponse(NotFound, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.NotFound,
                Exception = new EntityNotFoundException(request.UserID, nameof(User))
            });
        }

        return new GetUserDetailQueryResponse(userDetail);
    }
}

public class GetUserDetailQueryResponse : BaseResponse
{
    public GetUserDetailQueryResponse()
    {
    }

    public GetUserDetailQueryResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public GetUserDetailQueryResponse(UserDetail user)
    {
        Status = Ok;
        User = user;
        Failure = null;
    }

    public UserDetail? User { get; set; }
}
