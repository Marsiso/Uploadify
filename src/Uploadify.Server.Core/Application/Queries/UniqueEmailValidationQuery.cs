using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Localization;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static System.String;

namespace Uploadify.Server.Core.Application.Queries;

public class UniqueEmailValidationQuery : BaseRequest<UniqueEmailValidationQueryResponse>, IQuery<UniqueEmailValidationQueryResponse>
{
    public UniqueEmailValidationQuery(string? email)
    {
        Email = email;
    }

    public string? Email { get; set; }
}

public class UniqueEmailValidationQueryHandler : IQueryHandler<UniqueEmailValidationQuery, UniqueEmailValidationQueryResponse>
{
    private static readonly Func<DataContext, string, Task<bool>> Query = EF.CompileAsyncQuery((DataContext context, string email) =>
        context.Users.Any(user => user.NormalizedEmail == email));

    private readonly DataContext _context;
    private readonly ILookupNormalizer _normalizer;

    public UniqueEmailValidationQueryHandler(DataContext context, ILookupNormalizer normalizer)
    {
        _context = context;
        _normalizer = normalizer;
    }

    public async Task<UniqueEmailValidationQueryResponse> Handle(UniqueEmailValidationQuery request, CancellationToken cancellationToken)
    {
        if (IsNullOrWhiteSpace(request.Email))
        {
            return new UniqueEmailValidationQueryResponse(Status.BadRequest, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                Exception = new BadRequestException(nameof(request.Email))
            });
        }

        return new UniqueEmailValidationQueryResponse(!await Query(_context, _normalizer.NormalizeEmail(request.Email)));
    }
}

public class UniqueEmailValidationQueryResponse : BaseResponse
{
    public UniqueEmailValidationQueryResponse()
    {
    }

    public UniqueEmailValidationQueryResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public UniqueEmailValidationQueryResponse(bool isUnique)
    {
        Status = Status.Ok;
        IsUnique = isUnique;
        Failure = null;
    }

    public bool IsUnique { get; set; }
}
