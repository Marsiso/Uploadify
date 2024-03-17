using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Application.Models;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static System.String;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;

namespace Uploadify.Server.Core.Application.Queries;

public class GetUserQuery : IBaseRequest<GetUserQueryResponse>, IQuery<GetUserQueryResponse>
{
    public GetUserQuery(string? userName)
    {
        UserName = userName;
    }

    public string? UserName { get; set; }
}

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, GetUserQueryResponse>
{
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

        var normalizedUserName = _normalizer.NormalizeName(request.UserName);
        var user = await _context.Users
            .Include(user => user.Roles)
            !.ThenInclude(userRole => userRole.Role)
            .SingleOrDefaultAsync(user => user.NormalizedUserName == normalizedUserName, cancellationToken);

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

    public GetUserQueryResponse(User user) : base(Ok)
    {
        User = user;
    }

    public User? User { get; set; }
}
