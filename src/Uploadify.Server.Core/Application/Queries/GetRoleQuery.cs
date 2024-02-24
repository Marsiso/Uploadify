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

public class GetRoleQuery : BaseRequest<GetRoleQueryResponse>, IQuery<GetRoleQueryResponse>
{
    public GetRoleQuery(string? name)
    {
        Name = name;
    }

    public string? Name { get; set; }
}

public class GetRoleQueryHandler : IQueryHandler<GetRoleQuery, GetRoleQueryResponse>
{
    private static readonly Func<DataContext, string, Task<Role?>> Query = EF.CompileAsyncQuery((DataContext context, string name) =>
        context.Roles.SingleOrDefault(role => role.Name == name));

    private readonly DataContext _context;

    public GetRoleQueryHandler(DataContext context)
    {
        _context = context;
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

        var role = await Query(_context, request.Name);
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
    public GetRoleQueryResponse(Status status, RequestFailure failure) : base(status, failure)
    {
        Status = status;
        Failure = failure;
    }

    public GetRoleQueryResponse(Role role)
    {
        Status = Ok;
        Role = role;
    }

    public Role? Role { get; set; }
}
