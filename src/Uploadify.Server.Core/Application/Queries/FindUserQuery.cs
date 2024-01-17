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

namespace Uploadify.Server.Core.Application.Queries;

public class FindUserQuery : BaseRequest<FindUserQueryResponse>, IQuery<FindUserQueryResponse>
{
    public FindUserQuery(string? userName)
    {
        UserName = userName;
    }

    public string? UserName { get; set; }
}

public class FindUserQueryHandler : IQueryHandler<FindUserQuery, FindUserQueryResponse>
{
    private static readonly Func<DataContext, string, Task<User?>> Query = EF.CompileAsyncQuery((DataContext context, string username) =>
        context.Users.Where(user => user.NormalizedUserName == username)
            .Include(user => user.Roles)
            !.ThenInclude(userRole => userRole.Role)
            .SingleOrDefault());

    private readonly DataContext _context;
    private readonly ILookupNormalizer _normalizer;

    public FindUserQueryHandler(DataContext context, ILookupNormalizer normalizer)
    {
        _context = context;
        _normalizer = normalizer;
    }

    public async Task<FindUserQueryResponse> Handle(FindUserQuery request, CancellationToken cancellationToken)
    {
        if (IsNullOrWhiteSpace(request.UserName))
        {
            return new FindUserQueryResponse
            {
                Status = Status.BadRequest,
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
            return new FindUserQueryResponse(Status.NotFound, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.NotFound,
                Exception = new EntityNotFoundException(request.UserName, nameof(User))
            });
        }

        return new FindUserQueryResponse(user);
    }
}

public class FindUserQueryResponse : BaseResponse
{
    public FindUserQueryResponse()
    {
    }

    public FindUserQueryResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public FindUserQueryResponse(User user)
    {
        Status = Status.Ok;
        User = user;
    }

    public User? User { get; set; }
}
