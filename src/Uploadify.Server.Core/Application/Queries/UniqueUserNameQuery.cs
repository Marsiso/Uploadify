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
    private static readonly Func<DataContext, string, Task<bool>> Query = EF.CompileAsyncQuery((DataContext context, string username) =>
        context.Users.Any(user => user.NormalizedUserName == username));

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

        return new(!await Query(_context, _normalizer.NormalizeName(request.Username)));
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

    public UniqueUserNameQueryResponse(bool isUnique)
    {
        Status = Ok;
        IsUnique = isUnique;
    }

    public bool IsUnique { get; set; }
}
