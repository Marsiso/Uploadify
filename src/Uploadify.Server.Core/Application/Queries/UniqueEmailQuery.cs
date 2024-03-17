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

public class UniqueEmailQuery : IBaseRequest<UniqueEmailQueryResponse>, IQuery<UniqueEmailQueryResponse>
{
    public UniqueEmailQuery(string? email)
    {
        Email = email;
    }

    public string? Email { get; set; }
}

public class UniqueEmailQueryHandler : IQueryHandler<UniqueEmailQuery, UniqueEmailQueryResponse>
{
    private readonly DataContext _context;
    private readonly ILookupNormalizer _normalizer;

    public UniqueEmailQueryHandler(DataContext context, ILookupNormalizer normalizer)
    {
        _context = context;
        _normalizer = normalizer;
    }

    public async Task<UniqueEmailQueryResponse> Handle(UniqueEmailQuery request, CancellationToken cancellationToken)
    {
        if (IsNullOrWhiteSpace(request.Email))
        {
            return new(BadRequest, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                Exception = new BadRequestException(nameof(request.Email))
            });
        }

        var normalizedEmail = _normalizer.NormalizeEmail(request.Email);
        return new(! await _context.Users.AnyAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken: cancellationToken));
    }
}

public class UniqueEmailQueryResponse : BaseResponse
{
    public UniqueEmailQueryResponse()
    {
    }

    public UniqueEmailQueryResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public UniqueEmailQueryResponse(bool isUnique) : base(Ok)
    {
        IsUnique = isUnique;
    }

    public bool IsUnique { get; set; }
}
