using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Infrastructure.Localization.Constants;
using Uploadify.Server.Domain.Infrastructure.Requests.Contracts;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using static System.String;
using static Uploadify.Server.Domain.Infrastructure.Requests.Models.Status;

namespace Uploadify.Server.Core.Application.Queries;

public class UniqueUserNameQuery : IBaseRequest<UniqueUserNameQueryResponse>, IQuery<UniqueUserNameQueryResponse>
{
    public UniqueUserNameQuery(string? username)
    {
        Username = username;
    }

    public string? Username { get; set; }
}

public class UniqueUserNameQueryHandler : IQueryHandler<UniqueUserNameQuery, UniqueUserNameQueryResponse>
{
    private readonly DataContext _context;
    private readonly ILookupNormalizer _normalizer;

    public UniqueUserNameQueryHandler(DataContext context, ILookupNormalizer normalizer)
    {
        _context = context;
        _normalizer = normalizer;
    }

    public async Task<UniqueUserNameQueryResponse> Handle(UniqueUserNameQuery request, CancellationToken cancellationToken)
    {
        if (IsNullOrWhiteSpace(request.Username))
        {
            return new(BadRequest, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                Exception = new BadRequestException(nameof(request.Username))
            });
        }

        var normalizedUserName = _normalizer.NormalizeName(request.Username);
        return new(!await _context.Users.AnyAsync(user => user.NormalizedUserName == normalizedUserName, cancellationToken: cancellationToken));
    }
}

public class UniqueUserNameQueryResponse : BaseResponse
{
    public UniqueUserNameQueryResponse()
    {
    }

    public UniqueUserNameQueryResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public UniqueUserNameQueryResponse(bool isUnique) : base(Ok)
    {
        IsUnique = isUnique;
    }

    public bool IsUnique { get; set; }
}
