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

public class RoleQuery : BaseRequest<RoleQueryResponse>, IQuery<RoleQueryResponse>
{
    public RoleQuery(string? name)
    {
        Name = name;
    }

    public string? Name { get; set; }
}

public class RoleQueryHandler : IQueryHandler<RoleQuery, RoleQueryResponse>
{
    private static readonly Func<DataContext, string, Task<Role?>> Query = EF.CompileAsyncQuery((DataContext context, string name) =>
        context.Roles.SingleOrDefault(role => role.Name == name));

    private readonly DataContext _context;

    public RoleQueryHandler(DataContext context)
    {
        _context = context;
    }

    public async Task<RoleQueryResponse> Handle(RoleQuery request, CancellationToken cancellationToken)
    {
        if (IsNullOrWhiteSpace(request.Name))
        {
            return new RoleQueryResponse(BadRequest, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                Exception = new BadRequestException(nameof(request.Name))
            });
        }

        var role = await Query(_context, request.Name);
        if (role == null)
        {
            return new RoleQueryResponse(NotFound, new RequestFailure
            {
                UserFriendlyMessage = Translations.RequestStatuses.BadRequest,
                Exception = new EntityNotFoundException(request.Name, nameof(Role))
            });
        }

        return new RoleQueryResponse(role);
    }
}

public class RoleQueryResponse : BaseResponse
{
    public RoleQueryResponse(Status status, RequestFailure failure) : base(status, failure)
    {
        Status = status;
        Failure = failure;
    }

    public RoleQueryResponse(Role role)
    {
        Status = Ok;
        Role = role;
    }

    public Role? Role { get; set; }
}
