using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static System.String;
using static Uploadify.Server.Domain.Requests.Models.Status;

namespace Uploadify.Server.Core.Application.Queries;

public class UniqueEmailQuery : BaseRequest<UniqueEmailQueryResponse>, IQuery<UniqueEmailQueryResponse>
{
    public UniqueEmailQuery(string? email)
    {
        Email = email;
    }

    public string? Email { get; set; }
}

public class UniqueEmailQueryHandler : IQueryHandler<UniqueEmailQuery, UniqueEmailQueryResponse>
{
    private static readonly Func<DataContext, string, Task<bool>> Query = EF.CompileAsyncQuery((DataContext context, string email) =>
        context.Users.Any(user => user.NormalizedEmail == email));

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

        return new(!await Query(_context, _normalizer.NormalizeEmail(request.Email)));
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

    public UniqueEmailQueryResponse(bool isUnique)
    {
        Status = Ok;
        IsUnique = isUnique;
    }

    public bool IsUnique { get; set; }
}
