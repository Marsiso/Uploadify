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

public class UserDetailQuery : BaseRequest<UserDetailQueryResponse>, IQuery<UserDetailQueryResponse>
{
    public UserDetailQuery(string? userId)
    {
        UserID = userId;
    }

    public string? UserID { get; set; }
}

public class UserDetailQueryHandler : IQueryHandler<UserDetailQuery, UserDetailQueryResponse>
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

    public UserDetailQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<UserDetailQueryResponse> Handle(UserDetailQuery request, CancellationToken cancellationToken)
    {
        if (IsNullOrWhiteSpace(request.UserID))
        {
            return new UserDetailQueryResponse
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
            return new UserDetailQueryResponse(NotFound, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.NotFound,
                Exception = new EntityNotFoundException(request.UserID, nameof(User))
            });
        }

        return new UserDetailQueryResponse(userDetail);
    }
}

public class UserDetailQueryResponse : BaseResponse
{
    public UserDetailQueryResponse()
    {
    }

    public UserDetailQueryResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public UserDetailQueryResponse(UserDetail user)
    {
        Status = Ok;
        User = user;
    }

    public UserDetail? User { get; set; }
}
