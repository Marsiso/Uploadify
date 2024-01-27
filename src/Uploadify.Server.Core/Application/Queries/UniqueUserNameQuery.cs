using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Uploadify.Server.Data.Infrastructure.EF;
using Uploadify.Server.Domain.Localization;
using Uploadify.Server.Domain.Localization.Constants;
using Uploadify.Server.Domain.Requests.Exceptions;
using Uploadify.Server.Domain.Requests.Models;
using Uploadify.Server.Domain.Requests.Services;
using static System.String;
using static Uploadify.Server.Domain.Requests.Models.Status;

namespace Uploadify.Server.Core.Application.Queries;

public class UniqueUserNameQuery : BaseRequest<UniqueUserNameQueryResponse>, IQuery<UniqueUserNameQueryResponse>
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
            return new UniqueUserNameQueryResponse(BadRequest, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                Exception = new BadRequestException(nameof(request.Username))
            });
        }

        return new UniqueUserNameQueryResponse(!await Query(_context, _normalizer.NormalizeName(request.Username)));
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
