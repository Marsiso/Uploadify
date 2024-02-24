using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static System.String;
using static Uploadify.Server.Domain.Requests.Models.Status;

namespace Uploadify.Server.Core.Application.Queries;

public class GetUserQuery : BaseRequest<GetUserQueryResponse>, IQuery<GetUserQueryResponse>
{
    public GetUserQuery(string? userName)
    {
        UserName = userName;
    }

    public string? UserName { get; set; }
}

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, GetUserQueryResponse>
{
    private static readonly Func<DataContext, string, Task<User?>> Query = EF.CompileAsyncQuery((DataContext context, string username) =>
        context.Users.Where(user => user.NormalizedUserName == username)
            .Include(user => user.Roles)
            !.ThenInclude(userRole => userRole.Role)
            .SingleOrDefault());

    private readonly DataContext _context;
    private readonly ILookupNormalizer _normalizer;

    public GetUserQueryHandler(DataContext context, ILookupNormalizer normalizer)
    {
        _context = context;
        _normalizer = normalizer;
    }

    public async Task<GetUserQueryResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        if (IsNullOrWhiteSpace(request.UserName))
        {
            return new()
            {
                Status = BadRequest,
                Failure = new()
                {
                    UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                    Exception = new BadRequestException(nameof(request.UserName))
                }
            };
        }

        var user = await Query(_context, _normalizer.NormalizeName(request.UserName));
        if (user == null)
        {
            return new(NotFound, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.NotFound,
                Exception = new EntityNotFoundException(request.UserName, nameof(User))
            });
        }

        return new(user);
    }
}

public class GetUserQueryResponse : BaseResponse
{
    public GetUserQueryResponse()
    {
    }

    public GetUserQueryResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public GetUserQueryResponse(User user)
    {
        Status = Ok;
        User = user;
    }

    public User? User { get; set; }
}
