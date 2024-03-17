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

public class GetRoleQuery : IBaseRequest<GetRoleQueryResponse>, IQuery<GetRoleQueryResponse>
{
    public GetRoleQuery(string? name)
    {
        Name = name;
    }

    public string? Name { get; set; }
}

public class GetRoleQueryHandler : IQueryHandler<GetRoleQuery, GetRoleQueryResponse>
{
    private readonly DataContext _context;
    private readonly ILookupNormalizer _normalizer;

    public GetRoleQueryHandler(DataContext context, ILookupNormalizer normalizer)
    {
        _context = context;
        _normalizer = normalizer;
    }

    public async Task<GetRoleQueryResponse> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        if (IsNullOrWhiteSpace(request.Name))
        {
            return new(BadRequest, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                Exception = new BadRequestException(nameof(request.Name))
            });
        }

        var normalizerRoleName = _normalizer.NormalizeName(request.Name);
        var role = await _context.Roles.SingleOrDefaultAsync(role => role.NormalizedName == normalizerRoleName, cancellationToken: cancellationToken);
        if (role == null)
        {
            return new(NotFound, new()
            {
                UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                Exception = new EntityNotFoundException(request.Name, nameof(Role))
            });
        }

        return new(role);
    }
}

public class GetRoleQueryResponse : BaseResponse
{
    public GetRoleQueryResponse()
    {
    }

    public GetRoleQueryResponse(Status status, RequestFailure failure) : base(status, failure)
    {
    }

    public GetRoleQueryResponse(Role role) : base(Ok)
    {
        Role = role;
    }

    public Role? Role { get; set; }
}
