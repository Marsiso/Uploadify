using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Infrastructure.Localization;
using Uploadify.Server.Domain.Infrastructure.Requests.Exceptions;
using Uploadify.Server.Domain.Infrastructure.Requests.Models;
using Uploadify.Server.Domain.Infrastructure.Requests.Services;
using static System.String;

namespace Uploadify.Server.Core.Application.Queries;

public class UniqueUserNameValidationQuery : BaseRequest<UniqueUserNameValidationQueryResponse>, IQuery<UniqueUserNameValidationQueryResponse>
{
    public UniqueUserNameValidationQuery(string? username)
    {
        Username = username;
    }

    public string? Username { get; set; }
}

public class UniqueUserNameValidationQueryHandler : IQueryHandler<UniqueUserNameValidationQuery, UniqueUserNameValidationQueryResponse>
{
    private static readonly Func<DataContext, string, Task<bool>> Query = EF.CompileAsyncQuery((DataContext context, string username) =>
        context.Users.Any(user => user.NormalizedUserName == username));

    private readonly DataContext _context;
    private readonly ILookupNormalizer _normalizer;

    public UniqueUserNameValidationQueryHandler(DataContext context, ILookupNormalizer normalizer)
    {
        _context = context;
        _normalizer = normalizer;
    }

    public async Task<UniqueUserNameValidationQueryResponse> Handle(UniqueUserNameValidationQuery request, CancellationToken cancellationToken)
    {
        if (IsNullOrWhiteSpace(request.Username))
        {
            return new UniqueUserNameValidationQueryResponse(Status.BadRequest, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                Exception = new BadRequestException(nameof(request.Username))
            });
        }

        return new UniqueUserNameValidationQueryResponse(!await Query(_context, _normalizer.NormalizeName(request.Username)));
    }
}

public class UniqueUserNameValidationQueryResponse : BaseResponse
{
    public UniqueUserNameValidationQueryResponse()
    {
    }

    public UniqueUserNameValidationQueryResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public UniqueUserNameValidationQueryResponse(bool isUnique)
    {
        Status = Status.Ok;
        IsUnique = isUnique;
        Failure = null;
    }

    public bool IsUnique { get; set; }
}
