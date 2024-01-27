using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Localization;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static System.String;
using static Uploadify.Server.Domain.Requests.Models.Status;

namespace Uploadify.Server.Core.Application.Queries;

public class UserQuery : BaseRequest<UserQueryResponse>, IQuery<UserQueryResponse>
{
    public UserQuery(string? userName)
    {
        UserName = userName;
    }

    public string? UserName { get; set; }
}

public class UserQueryHandler : IQueryHandler<UserQuery, UserQueryResponse>
{
    private static readonly Func<DataContext, string, Task<User?>> Query = EF.CompileAsyncQuery((DataContext context, string username) =>
        context.Users.Where(user => user.NormalizedUserName == username)
            .Include(user => user.Roles)
            !.ThenInclude(userRole => userRole.Role)
            .SingleOrDefault());

    private readonly DataContext _context;
    private readonly ILookupNormalizer _normalizer;

    public UserQueryHandler(DataContext context, ILookupNormalizer normalizer)
    {
        _context = context;
        _normalizer = normalizer;
    }

    public async Task<UserQueryResponse> Handle(UserQuery request, CancellationToken cancellationToken)
    {
        if (IsNullOrWhiteSpace(request.UserName))
        {
            return new UserQueryResponse
            {
                Status = BadRequest,
                Failure = new RequestFailure
                {
                    UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                    Exception = new BadRequestException(nameof(request.UserName))
                }
            };
        }

        var user = await Query(_context, _normalizer.NormalizeName(request.UserName));
        if (user == null)
        {
            return new UserQueryResponse(NotFound, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.NotFound,
                Exception = new EntityNotFoundException(request.UserName, nameof(User))
            });
        }

        return new UserQueryResponse(user);
    }
}

public class UserQueryResponse : BaseResponse
{
    public UserQueryResponse()
    {
    }

    public UserQueryResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public UserQueryResponse(User user)
    {
        Status = Ok;
        User = user;
    }

    public User? User { get; set; }
}
